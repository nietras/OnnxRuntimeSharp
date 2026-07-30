using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtEnvironment : SafeHandle
{
    public OrtEnvironment(string logId = "OnnxRuntimeSharp")
        : base(nint.Zero, ownsHandle: true)
    {
        SetHandle((nint)Ort.CreateEnvironment(logId));
    }

    public override bool IsInvalid => handle == nint.Zero;

    protected override unsafe bool ReleaseHandle()
    {
        Ort.ReleaseEnv((Ort.OrtEnv*)handle);
        return true;
    }
}
