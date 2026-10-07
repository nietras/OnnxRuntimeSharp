using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public abstract class OrtSafeHandle<TOrtHandle> : SafeHandle
    where TOrtHandle : unmanaged, Ort.IOrtHandle<TOrtHandle>
{
    protected OrtSafeHandle(bool ownsHandle = true)
        : base(IntPtr.Zero, ownsHandle)
    { }

    public override bool IsInvalid => handle == IntPtr.Zero;

    internal TOrtHandle Handle => TOrtHandle.Cast(DangerousGetHandle());
}
