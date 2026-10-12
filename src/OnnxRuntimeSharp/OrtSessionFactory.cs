using System;
using System.Collections.Generic;

namespace OnnxRuntimeSharp;

public static class OrtSessionFactory
{
    static readonly IReadOnlyList<OrtEpConfig> CPUSingleThreadList = [OrtEpConfigs.CPUSingleThread];
    static readonly Lazy<bool> IsFastOrtEpConfigAvailableCache =
        new(() => OrtEpConfigs.ProbeExecutionProvider(OrtEnv.Instance(),
                    OrtEpConfigs.TensorRT).IsAvailable);

    public static OrtSession CreateOnCPUSingleThread(ReadOnlySpan<byte> model,
        int warmupRuns = 1, int warmupBatchSize = 1) =>
        Create(model, CPUSingleThreadList, warmupRuns, warmupBatchSize);

    public static OrtSession Create(ReadOnlySpan<byte> model,
        IReadOnlyList<OrtEpConfig> prioritizedOrtEpConfigsToTry,
        int warmupRuns = 1, int warmupBatchSize = 1)
    {
        var env = OrtEnv.Instance();
        var options = new OrtSessionOptions();
        // IMPORTANT: This must be ALL to ensure model executes fast enough
        options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_ENABLE_ALL);

        var availablePrioritizedOrtEpConfigs = OrtEpConfigs
            .FindAvailablePrioritizedExecutionProviders(
                prioritizedOrtEpConfigsToTry);
        if (availablePrioritizedOrtEpConfigs.Count > 0)
        {
            var bestAppendOrtEpConfig = availablePrioritizedOrtEpConfigs[0];
            bestAppendOrtEpConfig.Append(env, options);
        }

        // For TensorRT this takes a long time to finish since it has to
        // optimize graph and try different tactics/kernels
        var inference = new OrtSession(env, model, options);

        // Warmup the OrtSession with 1 or more runs to ensure it is fully
        // ready incl. all native dependencies loaded and to ensure first actual
        // inference is not slow. If warmupRuns < 1 this will be skipped.
        inference.Warmup(warmupRuns, warmupBatchSize);

        return inference;
    }

    public static bool IsFastOrtEpConfigAvailable() => IsFastOrtEpConfigAvailableCache.Value;
}
