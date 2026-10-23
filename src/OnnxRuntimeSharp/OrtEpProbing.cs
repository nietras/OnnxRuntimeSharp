using System;
using System.Collections.Generic;
using System.Linq;

namespace OnnxRuntimeSharp;

public static class OrtEpProbing
{
    // ONNX IR 8 / opset 13: sum = Add(first, second), each a float tensor of shape [1].
    // Both inputs are runtime values, so the computation cannot be constant-folded.
    internal static ReadOnlySpan<byte> ProbeModelOnnxBytes =>
    [
        0x08, 0x08, 0x12, 0x10, 0x4F, 0x6E, 0x6E, 0x78, 0x52, 0x75, 0x6E, 0x74, 0x69, 0x6D, 0x65, 0x53,
        0x68, 0x61, 0x72, 0x70, 0x3A, 0x5E, 0x0A, 0x19, 0x0A, 0x05, 0x66, 0x69, 0x72, 0x73, 0x74, 0x0A,
        0x06, 0x73, 0x65, 0x63, 0x6F, 0x6E, 0x64, 0x12, 0x03, 0x73, 0x75, 0x6D, 0x22, 0x03, 0x41, 0x64,
        0x64, 0x12, 0x03, 0x41, 0x64, 0x64, 0x5A, 0x13, 0x0A, 0x05, 0x66, 0x69, 0x72, 0x73, 0x74, 0x12,
        0x0A, 0x0A, 0x08, 0x08, 0x01, 0x12, 0x04, 0x0A, 0x02, 0x08, 0x01, 0x5A, 0x14, 0x0A, 0x06, 0x73,
        0x65, 0x63, 0x6F, 0x6E, 0x64, 0x12, 0x0A, 0x0A, 0x08, 0x08, 0x01, 0x12, 0x04, 0x0A, 0x02, 0x08,
        0x01, 0x62, 0x11, 0x0A, 0x03, 0x73, 0x75, 0x6D, 0x12, 0x0A, 0x0A, 0x08, 0x08, 0x01, 0x12, 0x04,
        0x0A, 0x02, 0x08, 0x01, 0x42, 0x02, 0x10, 0x0D,
    ];

    public static IReadOnlyList<OrtEpConfig> ProbeAvailablePrioritizedExecutionProviders(
        IReadOnlyList<OrtEpConfig>? prioritizedExecutionProvidersToTry = null)
    {
        var env = OrtEnv.Instance;
        return ProbeAvailablePrioritizedExecutionProviders(env, prioritizedExecutionProvidersToTry);
    }

    /// <summary>Executes the smoke test eagerly and returns successful configurations in input order.</summary>
    public static IReadOnlyList<OrtEpConfig> ProbeAvailablePrioritizedExecutionProviders(
        OrtEnv env, IReadOnlyList<OrtEpConfig>? prioritizedExecutionProvidersToTry = null)
        => ProbeExecutionProviders(env, prioritizedExecutionProvidersToTry)
            .Where(result => result.IsAvailable).Select(result => result.Provider).ToArray();

    public static IReadOnlyList<OrtEpProbeResult> ProbeExecutionProviders(
        IReadOnlyList<OrtEpConfig>? prioritizedExecutionProvidersToTry = null)
    {
        var env = OrtEnv.Instance;
        return ProbeExecutionProviders(env, prioritizedExecutionProvidersToTry);
    }

    /// <summary>Tests each configuration with real inference and preserves failures for diagnostics.</summary>
    /// <remarks>
    /// Success applies only to this small float Add model, device and configuration, not every model.
    /// Non-CPU configurations are tested without CPU fallback. Native crashes cannot be caught.
    /// Custom append delegates must only call APIs supported by the loaded runtime.
    /// </remarks>
    public static IReadOnlyList<OrtEpProbeResult> ProbeExecutionProviders(
        OrtEnv env, IReadOnlyList<OrtEpConfig>? prioritizedExecutionProvidersToTry = null)
    {
        ArgumentNullException.ThrowIfNull(env);
        ObjectDisposedException.ThrowIf(env.IsClosed || env.IsInvalid, env);
        var providers = prioritizedExecutionProvidersToTry ?? OrtEpConfigs.DefaultPrioritizedList;
        var results = new List<OrtEpProbeResult>(providers.Count);
        foreach (var provider in providers)
        {
            var result = ProbeExecutionProvider(env, provider);
            results.Add(result);
        }
        return results.AsReadOnly();
    }

    public static OrtEpProbeResult ProbeExecutionProvider(OrtEpConfig config) =>
        ProbeExecutionProvider(OrtEnv.Instance, config);

    public static OrtEpProbeResult ProbeExecutionProvider(OrtEnv env, OrtEpConfig config)
    {
        // Older runtimes accept unknown config keys but do not enforce CPU-fallback disabling.
        RequireApiVersion(16);
        try
        {
            using var options = new OrtSessionOptions();
            options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_DISABLE_ALL);
            options.SetIntraOpThreadCount(1);
            options.SetInterOpThreadCount(1);
            if (!config.AllowsCpuFallback)
            {
                options.AddConfigEntry("session.disable_cpu_ep_fallback", "1");
            }
            config.Append(env, options);
            using var session = new OrtSession(env, ProbeModelOnnxBytes, options);
            RunProbe(session);
            return new(config, null);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            return new(config, exception);
        }
    }

    static void RunProbe(OrtSession session)
    {
        using var first = new OrtValue<float>([2], [1]);
        using var second = new OrtValue<float>([3], [1]);
        using var output = new OrtValue<float>([0], [1]);
        var inputs = new[] { session.CreateInputBinding(0, first), session.CreateInputBinding(1, second) };
        var outputs = new[] { session.CreateOutputBinding(0, output) };
        session.Run(inputs, outputs);
        if (output.Data[0] != 5f)
        {
            throw new InvalidOperationException(
                "Execution-provider probe returned an incorrect sum.");
        }
        first.Data[0] = -4;
        session.Run(inputs, outputs);
        if (output.Data[0] != -1f)
        {
            throw new InvalidOperationException(
                "Execution-provider probe returned an incorrect sum after changing the input.");
        }
    }

    static void RequireApiVersion(uint minimum)
    {
        if (Ort.ApiVersion < minimum)
        {
            throw new NotSupportedException(
                "This execution-provider configuration requires " +
                $"ONNX Runtime API {minimum} or later; " +
                $"loaded API is {Ort.ApiVersion}.");
        }
    }
}
