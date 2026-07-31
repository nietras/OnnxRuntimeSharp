using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed class OrtValueBinding
{
    internal OrtValueBinding(OrtTensorInfo info, SafeHandle value)
    {
        Info = info;
        Value = value;
    }

    public OrtTensorInfo Info { get; }

    internal nint NameHandle => Info.NameHandle;

    internal nint ValueHandle => Value.DangerousGetHandle();

    internal SafeHandle Value { get; }
}
