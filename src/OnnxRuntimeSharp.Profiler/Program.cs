using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using OnnxRuntimeSharp;

const string SearchPattern = "*.onnx";
const int BatchSize = 1;
const int WarmupCount = 3;
const int MinimumIterations = 10;
const int ProfilingSamples = 10;
const double TargetRunDurationMilliseconds = 1_000;
var concurrentTestDuration = TimeSpan.FromSeconds(1);
int[] concurrentThreadCountsToTest = []; //[1, 2, 4, 8, 16]; // SKIP CONCURRENT FOR NOW
string[] preferredExecutionProviders =
[
    "TensorrtExecutionProvider",
    "CUDAExecutionProvider",
    "DnnlExecutionProvider",
    "CPUExecutionProvider",
];

Action<string> log = message =>
{
    Console.WriteLine(message);
    Trace.WriteLine(message);
};

var workingDirectory = Environment.CurrentDirectory;
var modelPaths = Directory.GetFiles(workingDirectory, SearchPattern, SearchOption.AllDirectories);
Array.Sort(modelPaths, StringComparer.Ordinal);
AddNativeRuntimeDirectoryToPath();
var availableExecutionProviders = Ort.GetAvailableExecutionProviders();
var configurations = CreateConfigurations(availableExecutionProviders, preferredExecutionProviders);

log($"Current directory: '{workingDirectory}'");
log($"Found {modelPaths.Length} files for '{SearchPattern}': " +
    $"{string.Join(", ", modelPaths.Select(path => $"'{path}'"))}");
log($"Available execution providers: {string.Join(", ", availableExecutionProviders)}");

foreach (var modelPath in modelPaths)
{
    var reportPath = Path.Combine(
        Path.GetDirectoryName(modelPath)!,
        $"{Path.GetFileNameWithoutExtension(modelPath)}.onnxruntime-profiler.md");

    using var writer = new StreamWriter(reportPath);
    Action<string> report = message =>
    {
        log(message);
        writer.WriteLine(message);
    };

    report($"# `{Path.GetRelativePath(workingDirectory, modelPath)}` ({new FileInfo(modelPath).Length} bytes)");
    report(string.Empty);
    report("## Execution provider performance");
    report("```");
    report($"{"Execution Provider",-32};BatchSize;Create [ms];First [ms];Iterations;Mean/b [ms];Mean/s [ms]");
    var configurationToProfilingInfo = new List<(ProfilingConfiguration Configuration, NodeProfileReport Report)>();
    foreach (var configuration in configurations)
    {
        try
        {
            configurationToProfilingInfo.Add((configuration, RunModel(modelPath, configuration, report)));
        }
        catch (OrtException exception)
        {
            report($"{configuration.Name,-32};Unavailable: {exception.Message}");
        }
    }
    report("```");

    report(string.Empty);
    report("## Concurrent app-thread scaling (single shared session)");
    report("```");
    report($"{"Execution Provider",-32};Threads;Iterations;Throughput [calls/s];Min Mean/call [ms];Avg Mean/call [ms];Max Mean/call [ms]");
    foreach (var configuration in configurations)
    {
        try
        {
            RunModelConcurrent(modelPath, configuration, concurrentThreadCountsToTest, concurrentTestDuration, report);
        }
        catch (OrtException exception)
        {
            report($"{configuration.Name,-32};Unavailable: {exception.Message}");
        }
    }
    report("```");

    foreach (var (configuration, profileReport) in configurationToProfilingInfo)
    {
        if (profileReport.Profiles.Count > 0)
        {
            WriteNodeProfileSummary(configuration.Name, modelPath, profileReport, report);
        }
    }
    log($"Wrote report: '{reportPath}'.");
}

if (modelPaths.Length == 0)
{
    log($"No models found. Copy one or more '{SearchPattern}' files below '{workingDirectory}'.");
}

static NodeProfileReport RunModel(
    string modelPath,
    ProfilingConfiguration configuration,
    Action<string> log)
{
    var model = File.ReadAllBytes(modelPath);
    using var environment = new OrtEnvironment();
    var profilePrefix = configuration.EnableProfiling
        ? Path.Combine(
            Path.GetDirectoryName(modelPath)!,
            $"{Path.GetFileNameWithoutExtension(modelPath)}-onnxruntime-profile-{SanitizeFileName(configuration.Name)}")
        : null;
    using var options = CreateSessionOptions(configuration, profilePrefix);
    var beforeCreate = Stopwatch.GetTimestamp();
    using var session = new OrtSession(environment, model, options);
    var createMilliseconds = ElapsedMilliseconds(beforeCreate);
    using var inputs = CreateInputBindings(session);
    using var outputs = CreateOutputBindings(session);

    var beforeFirstInference = Stopwatch.GetTimestamp();
    session.Run(inputs.Values, outputs.Values);
    var firstInferenceMilliseconds = ElapsedMilliseconds(beforeFirstInference);

    for (var warmup = 0; warmup < WarmupCount; ++warmup)
    {
        inputs.Tensors[0].Data[0] = warmup;
        session.Run(inputs.Values, outputs.Values);
        _ = outputs.Tensors[0].Data[0];
    }

    var iterations = 0;
    var totalMilliseconds = 0.0;
    var allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();
    while (totalMilliseconds < TargetRunDurationMilliseconds || iterations < MinimumIterations)
    {
        inputs.Tensors[0].Data[0] = iterations;
        var beforeInference = Stopwatch.GetTimestamp();
        session.Run(inputs.Values, outputs.Values);
        _ = outputs.Tensors[0].Data[0];
        totalMilliseconds += ElapsedMilliseconds(beforeInference);
        ++iterations;
    }
    var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore;

    var meanPerBatchMilliseconds = totalMilliseconds / iterations;
    log($"{configuration.Name,-32};{BatchSize,9};{createMilliseconds,11:F3};{firstInferenceMilliseconds,10:F3};" +
        $"{iterations,10};{meanPerBatchMilliseconds,11:F3};{meanPerBatchMilliseconds / BatchSize,11:F3}");
    if (allocatedBytes != 0)
    {
        log($"WARNING: `{configuration.Name}` single-request inference allocated {allocatedBytes} managed bytes.");
    }

    if (!configuration.EnableProfiling)
    {
        return new(null, []);
    }

    for (var sample = 0; sample < ProfilingSamples; ++sample)
    {
        inputs.Tensors[0].Data[0] = sample;
        session.Run(inputs.Values, outputs.Values);
    }

    var profilePath = session.EndProfiling();
    log($"Wrote ONNX Runtime trace: '{profilePath}'.");
    return new(profilePath, ReadNodeProfiles(profilePath));
}

static void RunModelConcurrent(
    string modelPath,
    ProfilingConfiguration configuration,
    int[] threadCounts,
    TimeSpan duration,
    Action<string> log)
{
    var model = File.ReadAllBytes(modelPath);
    foreach (var threadCount in threadCounts)
    {
        using var environment = new OrtEnvironment();
        using var options = CreateSessionOptions(configuration, null);
        using var session = new OrtSession(environment, model, options);
        using var barrier = new Barrier(threadCount + 1);
        var iterationsPerThread = new long[threadCount];
        var totalMillisecondsPerThread = new double[threadCount];
        var allocatedBytesPerThread = new long[threadCount];
        var running = 1;
        OrtException? failure = null;
        var threads = new Thread[threadCount];

        for (var threadIndex = 0; threadIndex < threadCount; ++threadIndex)
        {
            var index = threadIndex;
            threads[index] = new Thread(() =>
            {
                var barrierSignaled = false;
                try
                {
                    using var inputs = CreateInputBindings(session);
                    using var outputs = CreateOutputBindings(session);
                    for (var warmup = 0; warmup < WarmupCount; ++warmup)
                    {
                        inputs.Tensors[0].Data[0] = warmup;
                        session.Run(inputs.Values, outputs.Values);
                        _ = outputs.Tensors[0].Data[0];
                    }

                    barrier.SignalAndWait();
                    barrierSignaled = true;
                    _ = GC.GetAllocatedBytesForCurrentThread();
                    inputs.Tensors[0].Data[0] = 0;
                    session.Run(inputs.Values, outputs.Values);
                    _ = outputs.Tensors[0].Data[0];

                    var iterations = 0L;
                    var totalMilliseconds = 0.0;
                    var allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();
                    while (Volatile.Read(ref running) != 0)
                    {
                        inputs.Tensors[0].Data[0] = iterations;
                        var beforeInference = Stopwatch.GetTimestamp();
                        session.Run(inputs.Values, outputs.Values);
                        _ = outputs.Tensors[0].Data[0];
                        totalMilliseconds += ElapsedMilliseconds(beforeInference);
                        ++iterations;
                    }
                    iterationsPerThread[index] = iterations;
                    totalMillisecondsPerThread[index] = totalMilliseconds;
                    allocatedBytesPerThread[index] = GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore;
                }
                catch (OrtException exception)
                {
                    if (!barrierSignaled)
                    {
                        barrier.SignalAndWait();
                    }
                    Interlocked.CompareExchange(ref failure, exception, null);
                    Volatile.Write(ref running, 0);
                }
            })
            {
                IsBackground = true,
            };
            threads[index].Start();
        }

        barrier.SignalAndWait();
        var beforeAllInferences = Stopwatch.GetTimestamp();
        Thread.Sleep(duration);
        Volatile.Write(ref running, 0);

        foreach (var thread in threads)
        {
            thread.Join();
        }
        if (failure is not null)
        {
            throw failure;
        }

        var elapsedMilliseconds = ElapsedMilliseconds(beforeAllInferences);
        var totalIterations = iterationsPerThread.Sum();
        var meanCallMilliseconds = Enumerable.Range(0, threadCount)
            .Select(index => iterationsPerThread[index] == 0
                ? 0.0
                : totalMillisecondsPerThread[index] / iterationsPerThread[index])
            .ToArray();
        var throughputPerSecond = totalIterations / (elapsedMilliseconds / 1_000.0);

        log($"{configuration.Name,-32};{threadCount,7};{totalIterations,10};{throughputPerSecond,20:F1};" +
            $"{meanCallMilliseconds.Min(),18:F3};{meanCallMilliseconds.Average(),18:F3};{meanCallMilliseconds.Max(),18:F3}");
        if (allocatedBytesPerThread.Any(allocatedBytes => allocatedBytes != 0))
        {
            log($"WARNING: `{configuration.Name}` concurrent inference with {threadCount} threads allocated " +
                $"managed bytes per thread: {string.Join(", ", allocatedBytesPerThread)}.");
        }
    }
}

static OrtSessionOptions CreateSessionOptions(ProfilingConfiguration configuration, string? profilePrefix)
{
    var options = new OrtSessionOptions();
    if (configuration.IntraOpThreadCount is { } threadCount)
    {
        options.SetIntraOpThreadCount(threadCount);
    }
    if (configuration.InterOpThreadCount is { } interOpThreadCount)
    {
        options.SetInterOpThreadCount(interOpThreadCount);
    }
    options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_ENABLE_ALL);
    if (configuration.ProviderName is { } providerName)
    {
        options.AppendExecutionProvider(providerName);
    }
    if (profilePrefix is not null)
    {
        options.EnableProfiling(profilePrefix ?? throw new ArgumentNullException(nameof(profilePrefix)));
    }
    return options;
}

static IReadOnlyList<NodeProfile> ReadNodeProfiles(string profilePath)
{
    using var document = JsonDocument.Parse(File.ReadAllBytes(profilePath));
    var profiles = new Dictionary<string, NodeProfile>(StringComparer.Ordinal);
    foreach (var traceEvent in document.RootElement.EnumerateArray())
    {
        if (!traceEvent.TryGetProperty("cat", out var category) ||
            !string.Equals(category.GetString(), "Node", StringComparison.Ordinal) ||
            !traceEvent.TryGetProperty("dur", out var duration))
        {
            continue;
        }

        var nodeName = traceEvent.GetProperty("name").GetString();
        if (nodeName is null || !nodeName.EndsWith("_kernel_time", StringComparison.Ordinal))
        {
            continue;
        }

        nodeName = nodeName[..^"_kernel_time".Length];
        if (!profiles.TryGetValue(nodeName, out var profile))
        {
            profile = new NodeProfile(nodeName);
            profiles.Add(nodeName, profile);
        }
        profile.Add(duration.GetDouble());
    }
    return profiles.Values.OrderBy(profile => profile.NodeName, StringComparer.Ordinal).ToArray();
}

static void WriteNodeProfileSummary(
    string configurationName,
    string modelPath,
    NodeProfileReport profileReport,
    Action<string> log)
{
    const string NodeHeader = "Node";
    const string CallsHeader = "Calls";
    const string TotalMillisecondsHeader = "Total [ms]";
    const string MeanMillisecondsHeader = "Mean [ms/call]";
    const int CallsWidth = 5;
    const int TotalMillisecondsWidth = 10;
    const int MeanMillisecondsWidth = 13;

    var nodeWidth = Math.Max(NodeHeader.Length, profileReport.Profiles.Max(item => item.NodeName.Length));
    var headerFormat = $"{{0,-{nodeWidth}}};{{1,{CallsWidth}}};{{2,{TotalMillisecondsWidth}}};{{3,{MeanMillisecondsWidth}}}";
    var rowFormat = $"{{0,-{nodeWidth}}};{{1,{CallsWidth}}};{{2,{TotalMillisecondsWidth}:F3}};{{3,{MeanMillisecondsWidth}:F3}}";

    log(string.Empty);
    log($"## CPU node profile: `{configurationName}`");
    log($"Model: `{Path.GetFileName(modelPath)}`");
    log($"Source trace: `{Path.GetFileName(profileReport.TracePath)}`");
    log("```");
    log(string.Format(null, headerFormat,
        NodeHeader, CallsHeader, TotalMillisecondsHeader, MeanMillisecondsHeader));
    foreach (var item in profileReport.Profiles)
    {
        log(string.Format(null, rowFormat,
            item.NodeName, item.CallCount, item.TotalMicroseconds / 1_000.0, item.MeanMilliseconds));
    }
    log("```");
}

static OrtTensor<float> CreateFloatTensor(ReadOnlySpan<long> dimensions) =>
    new(new float[GetTensorElementCount(dimensions)], dimensions);

static TensorBindings CreateInputBindings(OrtSession session) =>
    CreateBindings(session.Inputs, session.CreateInputBinding);

static TensorBindings CreateOutputBindings(OrtSession session) =>
    CreateBindings(session.Outputs, session.CreateOutputBinding);

static TensorBindings CreateBindings(
    IReadOnlyList<OrtTensorInfo> infos,
    Func<int, OrtTensor<float>, OrtValueBinding> createBinding)
{
    var tensors = new OrtTensor<float>[infos.Count];
    var values = new OrtValueBinding[infos.Count];
    try
    {
        for (var index = 0; index < tensors.Length; ++index)
        {
            if (infos[index].ElementType != Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT)
            {
                throw new NotSupportedException(
                    $"The profiler supports float tensors only. '{infos[index].Name}' is {infos[index].ElementType}.");
            }

            tensors[index] = CreateFloatTensor(infos[index].Dimensions.Span);
            values[index] = createBinding(index, tensors[index]);
        }
        return new(tensors, values);
    }
    catch
    {
        foreach (var tensor in tensors)
        {
            tensor?.Dispose();
        }
        throw;
    }
}

static int GetTensorElementCount(ReadOnlySpan<long> dimensions)
{
    long elementCount = 1;
    foreach (var dimension in dimensions)
    {
        if (dimension <= 0)
        {
            throw new NotSupportedException("The profiler requires model inputs and outputs with fixed, positive dimensions.");
        }

        checked
        {
            elementCount *= dimension;
        }
    }

    return checked((int)elementCount);
}

static double ElapsedMilliseconds(long beforeTimestamp) =>
    (Stopwatch.GetTimestamp() - beforeTimestamp) * 1_000.0 / Stopwatch.Frequency;

static string SanitizeFileName(string value) =>
    string.Concat(value.Select(character =>
        Path.GetInvalidFileNameChars().Contains(character) ? '_' : character));

static void AddNativeRuntimeDirectoryToPath()
{
    if (!OperatingSystem.IsWindows())
    {
        return;
    }

    var nativeRuntimeDirectory = Path.Combine(AppContext.BaseDirectory, "runtimes", "win-x64", "native");
    if (!Directory.Exists(nativeRuntimeDirectory))
    {
        return;
    }

    var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
    if (path.Split(Path.PathSeparator).Contains(nativeRuntimeDirectory, StringComparer.OrdinalIgnoreCase))
    {
        return;
    }

    Environment.SetEnvironmentVariable(
        "PATH",
        string.Concat(nativeRuntimeDirectory, Path.PathSeparator, path),
        EnvironmentVariableTarget.Process);
}

static ProfilingConfiguration[] CreateConfigurations(
    IReadOnlyList<string> availableExecutionProviders,
    IReadOnlyList<string> preferredExecutionProviders)
{
    var configurations = new List<ProfilingConfiguration>();
    foreach (var providerName in preferredExecutionProviders)
    {
        if (!availableExecutionProviders.Contains(providerName, StringComparer.Ordinal))
        {
            continue;
        }

        if (string.Equals(providerName, "CPUExecutionProvider", StringComparison.Ordinal))
        {
            configurations.Add(new("CPU", null, null, null, false));
            configurations.Add(new("CPU 1×Intra 1×Inter", null, 1, 1, true));
        }
        else
        {
            configurations.Add(new(providerName, providerName, null, null, false));
        }
    }

    return [.. configurations];
}

sealed record ProfilingConfiguration(
    string Name,
    string? ProviderName,
    int? IntraOpThreadCount,
    int? InterOpThreadCount,
    bool EnableProfiling);

sealed record NodeProfileReport(string? TracePath, IReadOnlyList<NodeProfile> Profiles);

sealed class TensorBindings(OrtTensor<float>[] tensors, OrtValueBinding[] values) : IDisposable
{
    public OrtTensor<float>[] Tensors { get; } = tensors;
    public OrtValueBinding[] Values { get; } = values;

    public void Dispose()
    {
        foreach (var tensor in Tensors)
        {
            tensor.Dispose();
        }
    }
}

sealed class NodeProfile(string nodeName)
{
    public string NodeName { get; } = nodeName;
    public int CallCount { get; private set; }
    public double TotalMicroseconds { get; private set; }
    public double MeanMilliseconds => TotalMicroseconds / 1_000.0 / CallCount;

    public void Add(double durationMicroseconds)
    {
        ++CallCount;
        TotalMicroseconds += durationMicroseconds;
    }
}
