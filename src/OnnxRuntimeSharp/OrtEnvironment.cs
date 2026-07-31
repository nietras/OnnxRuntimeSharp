using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtEnvironment : SafeHandle
{
    public OrtEnvironment(string logId = "OnnxRuntimeSharp")
        : base(IntPtr.Zero, ownsHandle: true)
    {
        SetHandle((IntPtr)Ort.CreateEnvironment(logId));
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override unsafe bool ReleaseHandle()
    {
        Ort.ReleaseEnv((Ort.OrtEnv*)handle);
        return true;
    }
}
