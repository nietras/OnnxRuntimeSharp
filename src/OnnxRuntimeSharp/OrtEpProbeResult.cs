using System;

namespace OnnxRuntimeSharp;

public readonly record struct OrtEpProbeResult(OrtEpConfig Provider, Exception? Error)
{
    public bool IsAvailable => Error is null;
}
