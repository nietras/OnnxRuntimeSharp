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
var concurrentTestDuration = TimeSpan.FromSeconds(1);
int[] concurrentThreadCountsToTest = [1, 2, 4, 8, 16]; // SKIP CONCURRENT FOR NOW
var configurations = new Dictionary<string, Action<OrtSessionOptions>>
{
    //["TensorRT"] = options =>
    //{
    //    options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_ENABLE_ALL);
    //    options.AppendTensorRtExecutionProvider();
    //},
    //["CUDA"] = options =>
    //{
    //    options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_ENABLE_ALL);
    //    options.AppendCudaExecutionProvider();
    //},
    //["OpenVINO"] = options =>
    //{
    //    options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_DISABLE_ALL);
    //    options.AppendOpenVinoExecutionProvider();
    //},
    //["OpenVINO Throughput"] = options =>
    //{
    //    options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_DISABLE_ALL);
    //    options.AppendOpenVinoExecutionProvider(new Dictionary<string, string>
    //    {
    //        { "device_type", "CPU" },
    //        { "load_config", "{\"CPU\":{\"PERFORMANCE_HINT\":\"THROUGHPUT\"}}" },
    //    });
    //},
    //["OpenVINO 1×Threads 1×Streams"] = options =>
    //{
    //    options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_DISABLE_ALL);
    //    options.AppendOpenVinoExecutionProvider(new Dictionary<string, string>
    //    {
    //        { "device_type", "CPU" },
    //        { "num_of_threads", "1" },
    //        { "num_streams", "1" },
    //    });
    //},
    ["OpenVINO 16×Threads 8×Streams"] = options =>
    {
        options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_DISABLE_ALL);
        options.AppendOpenVinoExecutionProvider(new Dictionary<string, string>
        {
            { "device_type", "CPU" },
            { "num_of_threads", "16" },
            { "num_streams", "8" },
        });
    },
    //["CPU"] = options => options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_ENABLE_ALL),
    //["CPU 2×Intra 16×Inter"] = options =>
    //{
    //    options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_ENABLE_ALL);
    //    options.SetIntraOpThreadCount(2);
    //    options.SetInterOpThreadCount(16);
    //},
    //["CPU 1×Intra 1×Inter"] = options =>
    //{
    //    options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_ENABLE_ALL);
    //    options.SetIntraOpThreadCount(1);
    //    options.SetInterOpThreadCount(1);
    //},
};

Action<string> log = message =>
{
    Console.WriteLine(message);
    Trace.WriteLine(message);
};

var workingDirectory = Environment.CurrentDirectory;
var modelPaths = Directory.GetFiles(workingDirectory, SearchPattern, SearchOption.TopDirectoryOnly);
Array.Sort(modelPaths, StringComparer.Ordinal);
AddNativeRuntimeDirectoryToPath();
var availableExecutionProviders = Ort.GetAvailableExecutionProviders();

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
    var configurationToProfilingInfo = new List<(string Name, NodeProfileReport Report)>();
    foreach (var (configurationName, configureSessionOptions) in configurations)
    {
        try
        {
            configurationToProfilingInfo.Add((configurationName, RunModel(modelPath, configurationName, configureSessionOptions, report)));
        }
        catch (OrtException exception)
        {
            report($"{configurationName,-32};Unavailable: {exception.Message}");
        }
    }
    report("```");

    report(string.Empty);
    report("## Concurrent app-thread scaling (single shared session)");
    report("```");
    report($"{"Execution Provider",-32};Threads;Iterations;Throughput [calls/s];Min Mean/call [ms];Avg Mean/call [ms];Max Mean/call [ms]");
    foreach (var (configurationName, configureSessionOptions) in configurations)
    {
        try
        {
            RunModelConcurrent(modelPath, configurationName, configureSessionOptions, concurrentThreadCountsToTest, concurrentTestDuration, report);
        }
        catch (OrtException exception)
        {
            report($"{configurationName,-32};Unavailable: {exception.Message}");
        }
    }
    report("```");

    foreach (var (configurationName, profileReport) in configurationToProfilingInfo)
    {
        if (profileReport.Profiles.Count > 0)
        {
            WriteNodeProfileSummary(configurationName, modelPath, profileReport, report);
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
    string configurationName,
    Action<OrtSessionOptions> configureSessionOptions,
    Action<string> log)
{
    var model = File.ReadAllBytes(modelPath);
    using var environment = new OrtEnvironment();
    var profilePrefix = EnableProfiling
        ? Path.Combine(
            Path.GetDirectoryName(modelPath)!,
            $"{Path.GetFileNameWithoutExtension(modelPath)}-onnxruntime-profile-{SanitizeFileName(configurationName)}")
        : null;
    using var options = CreateSessionOptions(configureSessionOptions, profilePrefix);
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
    log($"{configurationName,-32};{BatchSize,9};{createMilliseconds,11:F3};{firstInferenceMilliseconds,10:F3};" +
        $"{iterations,10};{meanPerBatchMilliseconds,11:F3};{meanPerBatchMilliseconds / BatchSize,11:F3}");
    if (allocatedBytes != 0)
    {
        log($"WARNING: `{configurationName}` inference allocated {allocatedBytes} managed bytes.");
    }

    if (!EnableProfiling)
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
    string configurationName,
    Action<OrtSessionOptions> configureSessionOptions,
    int[] threadCounts,
    TimeSpan duration,
    Action<string> log)
{
    var model = File.ReadAllBytes(modelPath);
    foreach (var threadCount in threadCounts)
    {
        using var environment = new OrtEnvironment();
        using var options = CreateSessionOptions(configureSessionOptions, null);
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

        log($"{configurationName,-32};{threadCount,7};{totalIterations,10};{throughputPerSecond,20:F1};" +
            $"{meanCallMilliseconds.Min(),18:F3};{meanCallMilliseconds.Average(),18:F3};{meanCallMilliseconds.Max(),18:F3}");
        if (allocatedBytesPerThread.Any(allocatedBytes => allocatedBytes != 0))
        {
            log($"WARNING: `{configurationName}` concurrent inference with {threadCount} threads allocated " +
                $"managed bytes per thread: {string.Join(", ", allocatedBytesPerThread)}.");
        }
    }
}

static OrtSessionOptions CreateSessionOptions(
    Action<OrtSessionOptions> configureSessionOptions,
    string? profilePrefix)
{
    var options = new OrtSessionOptions();
    configureSessionOptions(options);
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
    string modelPath,
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
    log($"Model: `{Path.GetFileName(modelPath)}`");
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
