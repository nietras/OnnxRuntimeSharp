using System;

namespace OnnxRuntimeSharp;

/// <summary>A named execution-provider configuration that can be applied to a session.</summary>
public sealed class OrtEpConfig
{
    public OrtEpConfig(string name, Action<OrtSessionOptions> append, bool allowsCpuFallback = false)
        : this(name, Adapt(append), allowsCpuFallback)
    {
    }

    public OrtEpConfig(string name, Action<OrtEnv, OrtSessionOptions> append, bool allowsCpuFallback = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(append);
        Name = name;
        Append = append;
        AllowsCpuFallback = allowsCpuFallback;
    }

    public string Name { get; }
    public Action<OrtEnv, OrtSessionOptions> Append { get; }

    /// <summary>Whether the probe permits CPU execution. Set this for CPU configurations.</summary>
    public bool AllowsCpuFallback { get; }

    static Action<OrtEnv, OrtSessionOptions> Adapt(Action<OrtSessionOptions> append)
    {
        ArgumentNullException.ThrowIfNull(append);
        return (_, options) => append(options);
    }
}

public readonly record struct OrtEpProbeResult(OrtEpConfig Provider, Exception? Error)
{
    public bool IsAvailable => Error is null;
}
