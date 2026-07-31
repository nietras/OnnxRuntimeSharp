using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtValueBinding
{
    internal OrtValueBinding(OrtTensorInfo info, SafeHandle value)
    {
        Info = info;
        Value = value;
    }

    public OrtTensorInfo Info { get; }

    internal sbyte* NamePointer => Info.NamePointer;

    internal Ort.OrtValue* ValuePointer => (Ort.OrtValue*)Value.DangerousGetHandle();

    internal SafeHandle Value { get; }
}
