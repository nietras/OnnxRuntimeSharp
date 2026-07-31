using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed class OrtTensorInfo
{
    readonly nint _nameHandle;

    internal OrtTensorInfo(
        nint nameHandle,
        string name,
        long[] dimensions,
        Ort.ONNXTensorElementDataType elementType)
    {
        _nameHandle = nameHandle;
        Name = name;
        Dimensions = dimensions;
        ElementType = elementType;
    }

    public string Name { get; }

    public ReadOnlyMemory<long> Dimensions { get; }

    public Ort.ONNXTensorElementDataType ElementType { get; }

    internal nint NameHandle => _nameHandle;

    internal void Dispose() => Marshal.FreeCoTaskMem(_nameHandle);
}
