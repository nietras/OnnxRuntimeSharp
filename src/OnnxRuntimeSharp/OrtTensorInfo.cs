using System;
namespace OnnxRuntimeSharp;

public sealed unsafe class OrtTensorInfo
{
    readonly Ort.OrtAllocator* _allocator;
    sbyte* _name;

    internal OrtTensorInfo(
        Ort.OrtAllocator* allocator,
        sbyte* nativeName,
        string name,
        long[] dimensions,
        Ort.ONNXTensorElementDataType elementType)
    {
        _allocator = allocator;
        _name = nativeName;
        Name = name;
        Dimensions = dimensions;
        ElementType = elementType;
    }

    public string Name { get; }

    public ReadOnlyMemory<long> Dimensions { get; }

    public Ort.ONNXTensorElementDataType ElementType { get; }

    internal sbyte* NamePointer => _name;

    internal void Dispose()
    {
        if (_name is null)
        {
            return;
        }

        Ort.ReleaseAllocatorValue(_allocator, _name);
        _name = null;
    }
}
