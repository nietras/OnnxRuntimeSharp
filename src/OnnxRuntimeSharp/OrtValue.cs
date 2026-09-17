using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtValue : SafeHandle
{
    readonly long[] _dimensions;

    internal OrtValue(Ort.OrtValue* value)
        : base(IntPtr.Zero, ownsHandle: true)
    {
        if (value is null)
        {
            Throws.ThrowNativeValueNull();
        }

        SetHandle((IntPtr)value);
        Ort.OrtTensorTypeAndShapeInfo* tensorInfo;
        try
        {
            Ort.Ok(Ort.GetTensorTypeAndShape(value, &tensorInfo));
        }
        catch
        {
            Dispose();
            throw;
        }

        try
        {
            Ort.ONNXTensorElementDataType elementType;
            Ort.Ok(Ort.GetTensorElementType(tensorInfo, &elementType));
            ElementType = elementType;
            nuint dimensionCount;
            Ort.Ok(Ort.GetDimensionsCount(tensorInfo, &dimensionCount));
            _dimensions = new long[checked((int)dimensionCount)];
            fixed (long* dimensionsPointer = _dimensions)
            {
                Ort.Ok(Ort.GetDimensions(tensorInfo, dimensionsPointer, dimensionCount));
            }
        }
        catch
        {
            Dispose();
            throw;
        }
        finally
        {
            Ort.ReleaseTensorTypeAndShapeInfo(tensorInfo);
        }
    }

    public Ort.ONNXTensorElementDataType ElementType { get; }

    public ReadOnlyMemory<long> Dimensions => _dimensions;

    public Span<T> GetTensorData<T>() where T : unmanaged
    {
        ObjectDisposedException.ThrowIf(IsClosed || IsInvalid, this);
        var expectedType = OrtTensorElementType.Get<T>();
        if (ElementType != expectedType)
        {
            Throws.ThrowTensorElementTypeMismatch(ElementType, expectedType);
        }

        nuint elementCount = 1;
        foreach (var dimension in _dimensions)
        {
            elementCount = checked(elementCount * (nuint)dimension);
        }
        void* data;
        Ort.Ok(Ort.GetTensorMutableData((Ort.OrtValue*)handle, &data));
        return new Span<T>(data, checked((int)elementCount));
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    internal Ort.OrtValue* Pointer => (Ort.OrtValue*)handle;

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseValue(Pointer);
        return true;
    }
}
