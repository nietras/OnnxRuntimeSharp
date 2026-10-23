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
const bool EnableProfiling = false;
const double TargetRunDurationMilliseconds = 1_000;
const int EPAlign = -38;
var concurrentTestDuration = TimeSpan.FromSeconds(1);
int processorCount = Environment.ProcessorCount;
int[] concurrentThreadCountsToTest = [1, 2, 4, 8, 16, processorCount - 1];
OrtEpConfig[] configurations =
[
#if false && ENABLE_GPU_PROVIDERS
    OrtEpConfigs.TensorRT,
    OrtEpConfigs.CUDA,
#endif
    OrtEpConfigs.OpenVINO,
    OrtEpConfigs.OpenVINOCPUSingleThread,
    OrtEpConfigs.CreateOpenVINOCPU(threadCount: processorCount, streamCount: processorCount/2),
    OrtEpConfigs.CreateOpenVINOCPU(threadCount: 16, streamCount: 8),
    OrtEpConfigs.CreateOpenVINO("OpenVINO CPU 16×Threads 8×Streams bf16", new Dictionary<string, string>
        {
            { "device_type", "CPU" },
            { "num_of_threads", "16" },
            { "num_streams", "8" },
            { "load_config", "{\"CPU\":{\"INFERENCE_PRECISION_HINT\":\"bf16\"}}" },
        }),
    OrtEpConfigs.CPU,
    OrtEpConfigs.CPUSingleThread,
];

Action<string> log = message =>
{
    Console.WriteLine(message);
    Trace.WriteLine(message);
};

var workingDirectory = Environment.CurrentDirectory;
var models = Directory.GetFiles(workingDirectory, SearchPattern, SearchOption.TopDirectoryOnly)
    .Order(StringComparer.Ordinal)
    .Select(path => new ModelData(Path.GetFileName(path), Path.GetDirectoryName(path)!, () => File.ReadAllBytes(path)))
    .ToArray();
log($"Current directory: '{workingDirectory}'");
log($"Found {models.Length} files for '{SearchPattern}': " +
    $"{string.Join(", ", models.Select(model => $"'{model.FileName}'"))}");

if (models.Length == 0)
{
    const string MnistEmbeddedFileName = "mnist-8.onnx";
    log($"No ONNX models found. Run embedded {MnistEmbeddedFileName}? [y/N]");
    var answer = Console.ReadLine()?.Trim() ?? string.Empty;
    if (answer.StartsWith("y", StringComparison.OrdinalIgnoreCase))
    {
        var embeddedModelStream = (UnmanagedMemoryStream)(typeof(ModelData).Assembly
            .GetManifestResourceStream(MnistEmbeddedFileName)
            ?? throw new InvalidOperationException($"Embedded {MnistEmbeddedFileName} resource was not found."));
        models = [new(MnistEmbeddedFileName, AppContext.BaseDirectory,
                      () => GetEmbeddedModelBytes(embeddedModelStream), IsEmbedded: true)];
    }
    else
    {
        log("No model selected. Exiting.");
        return;
    }
}

AddNativeRuntimeDirectoryToPath();
var probeResults = OrtEpConfigs.ProbeExecutionProviders(configurations);
var availableConfigurations = probeResults.Where(result => result.IsAvailable)
    .Select(result => result.Provider).ToArray();
var availableConfigurationNames = availableConfigurations.Select(configuration => configuration.Name);
log($"Available configurations: {string.Join(", ", availableConfigurationNames)}");
foreach (var result in probeResults.Where(result => !result.IsAvailable))
{
    log($"{result.Provider.Name,EPAlign};Unavailable: {result.Error?.Message}");
}

foreach (var modelData in models)
{
    var model = modelData.GetBytes();
    var reportPath = Path.Combine(
        modelData.DirectoryName,
        $"{Path.GetFileNameWithoutExtension(modelData.FileName)}.onnxruntime-profiler.md");

    using var writer = new StreamWriter(reportPath);
    Action<string> report = message =>
    {
        log(message);
        writer.WriteLine(message);
    };

    report($"# `{modelData.FileName}` ({model.Length} bytes){(modelData.IsEmbedded ? " (embedded)" : string.Empty)}");
    report(string.Empty);
    report("## Execution provider performance");
    report("```");
    report($"{"Execution Provider",EPAlign};BatchSize;Create [ms];First [ms];Iterations;Mean/b [ms];Mean/s [ms]");
    var configurationToProfilingInfo = new List<(string Name, NodeProfileReport Report)>();
    foreach (var configuration in availableConfigurations)
    {
        try
        {
            var profilePrefix = EnableProfiling
                ? Path.Combine(modelData.DirectoryName,
                    $"{Path.GetFileNameWithoutExtension(modelData.FileName)}-onnxruntime-profile-{SanitizeFileName(configuration.Name)}")
                : null;
            var profileReport = RunModel(model, configuration, report, profilePrefix);
            configurationToProfilingInfo.Add((configuration.Name, profileReport));
        }
        catch (OrtException exception)
        {
            report($"{configuration.Name,EPAlign};Unavailable: {exception.Message}");
        }
    }
    report("```");

    report(string.Empty);
    report("## Concurrent app-thread scaling (single shared session)");
    report("```");
    report($"{"Execution Provider",EPAlign};Threads;Iterations;Throughput [calls/s];Min Mean/call [ms];Avg Mean/call [ms];Max Mean/call [ms]");
    foreach (var configuration in availableConfigurations)
    {
        try
        {
            RunModelConcurrent(model, configuration, concurrentThreadCountsToTest, concurrentTestDuration, report);
        }
        catch (OrtException exception)
        {
            report($"{configuration.Name,EPAlign};Unavailable: {exception.Message}");
        }
    }
    report("```");

    foreach (var (configurationName, profileReport) in configurationToProfilingInfo)
    {
        if (profileReport.Profiles.Count > 0)
        {
            WriteNodeProfileSummary(configurationName, modelData.FileName, profileReport, report);
        }
    }
    log($"Wrote report: '{reportPath}'.");
}

static unsafe ReadOnlySpan<byte> GetEmbeddedModelBytes(UnmanagedMemoryStream stream) =>
    new(stream.PositionPointer, checked((int)stream.Length));

static NodeProfileReport RunModel(
    ReadOnlySpan<byte> model,
    OrtEpConfig configuration,
    Action<string> log,
    string? profilePrefix)
{
    var env = OrtEnv.Instance();
    var configurationName = configuration.Name;
    using var options = CreateSessionOptions(env, configuration, profilePrefix);
    var beforeCreate = Stopwatch.GetTimestamp();
    using var session = new OrtSession(env, model, options);
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
    log($"{configurationName,EPAlign};{BatchSize,9};{createMilliseconds,11:F3};{firstInferenceMilliseconds,10:F3};" +
        $"{iterations,10};{meanPerBatchMilliseconds,11:F3};{meanPerBatchMilliseconds / BatchSize,11:F3}");
    if (allocatedBytes != 0)
    {
        log($"WARNING: `{configurationName}` inference allocated {allocatedBytes} managed bytes.");
    }

    if (profilePrefix is null)
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
    ReadOnlySpan<byte> model,
    OrtEpConfig configuration,
    int[] threadCounts,
    TimeSpan duration,
    Action<string> log)
{
    using var env = new OrtEnv();
    var configurationName = configuration.Name;
    using var options = CreateSessionOptions(env, configuration, null);
    using var session = new OrtSession(env, model, options);

    foreach (var threadCount in threadCounts)
    {
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

        log($"{configurationName,EPAlign};{threadCount,7};{totalIterations,10};{throughputPerSecond,20:F1};" +
            $"{meanCallMilliseconds.Min(),18:F3};{meanCallMilliseconds.Average(),18:F3};{meanCallMilliseconds.Max(),18:F3}");
        if (allocatedBytesPerThread.Any(allocatedBytes => allocatedBytes != 0))
        {
            log($"WARNING: `{configurationName}` concurrent inference with {threadCount} threads allocated " +
                $"managed bytes per thread: {string.Join(", ", allocatedBytesPerThread)}.");
        }
    }
}

static OrtSessionOptions CreateSessionOptions(OrtEnv env, OrtEpConfig configuration, string? profilePrefix)
{
    var options = new OrtSessionOptions();
    // Preserve profiler optimization defaults; OpenVINO configurations override this.
    options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_ENABLE_ALL);
    configuration.Append(env, options);
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
    var orderedProfiles = new List<NodeProfile>();
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
            orderedProfiles.Add(profile);
        }
        profile.Add(duration.GetDouble());
    }
    return orderedProfiles;
}

static void WriteNodeProfileSummary(
    string configurationName,
    string modelFileName,
    NodeProfileReport profileReport,
    Action<string> log)
{
    const string NodeHeader = "Node";
    const string CallsHeader = "Calls";
    const string TotalMillisecondsHeader = "Total [ms]";
    const string MeanMillisecondsHeader = "Mean [ms/call]";
    const string RatioHeader = "Ratio";
    const int CallsWidth = 5;
    const int TotalMillisecondsWidth = 10;
    const int MeanMillisecondsWidth = 13;
    const int RatioWidth = 5;

    var nodeWidth = Math.Max("Total".Length,
        Math.Max(NodeHeader.Length, profileReport.Profiles.Max(item => item.NodeName.Length)));
    var headerFormat = $"{{0,-{nodeWidth}}};{{1,{CallsWidth}}};{{2,{TotalMillisecondsWidth}}};{{3,{MeanMillisecondsWidth}}};{{4,{RatioWidth}}}";
    var rowFormat = $"{{0,-{nodeWidth}}};{{1,{CallsWidth}}};{{2,{TotalMillisecondsWidth}:F3}};{{3,{MeanMillisecondsWidth}:F3}};{{4,{RatioWidth}:F3}}";
    var totalMicroseconds = profileReport.Profiles.Sum(item => item.TotalMicroseconds);
    var totalCalls = profileReport.Profiles.Sum(item => item.CallCount);
    var totalMilliseconds = totalMicroseconds / 1_000.0;
    var totalMeanMilliseconds = totalCalls == 0 ? 0.0 : totalMilliseconds / totalCalls;

    log(string.Empty);
    log($"## CPU node profile: `{configurationName}`");
    log($"Model: `{modelFileName}`");
    log($"Source trace: `{Path.GetFileName(profileReport.TracePath)}`");
    log("```");
    log(string.Format(null, headerFormat,
        NodeHeader, CallsHeader, TotalMillisecondsHeader, MeanMillisecondsHeader, RatioHeader));
    foreach (var item in profileReport.Profiles)
    {
        var computeRatio = totalMicroseconds == 0.0 ? 0.0 : item.TotalMicroseconds / totalMicroseconds;
        log(string.Format(null, rowFormat,
            item.NodeName, item.CallCount, item.TotalMicroseconds / 1_000.0, item.MeanMilliseconds, computeRatio));
    }
    log(string.Format(null, rowFormat, "Total", totalCalls, totalMilliseconds, totalMeanMilliseconds, 1.0));
    log("```");
}

static OrtValue<float> CreateFloatTensor(ReadOnlySpan<long> dimensions) =>
    new(new float[GetTensorElementCount(dimensions)], dimensions);

static TensorBindings CreateInputBindings(OrtSession session) =>
    CreateBindings(session.Inputs.Span, session.CreateInputBinding);

static TensorBindings CreateOutputBindings(OrtSession session) =>
    CreateBindings(session.Outputs.Span, session.CreateOutputBinding);

static TensorBindings CreateBindings(
    ReadOnlySpan<OrtTensorInfo> infos,
    Func<int, OrtValue<float>, OrtValueBinding> createBinding)
{
    var tensors = new OrtValue<float>[infos.Length];
    var values = new OrtValueBinding[infos.Length];
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

sealed record ModelData(string FileName, string DirectoryName, Func<ReadOnlySpan<byte>> GetBytes, bool IsEmbedded = false);

sealed record NodeProfileReport(string? TracePath, IReadOnlyList<NodeProfile> Profiles);

sealed class TensorBindings(OrtValue<float>[] tensors, OrtValueBinding[] values) : IDisposable
{
    public OrtValue<float>[] Tensors { get; } = tensors;
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
