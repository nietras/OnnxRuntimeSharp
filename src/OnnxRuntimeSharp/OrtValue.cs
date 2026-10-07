using System;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtValue : OrtSafeHandle<Ort.OrtValueHandle>
{
    readonly long[] _dimensions;

    internal OrtValue(Ort.OrtValueHandle value)
    {
        if (value.IsNull)
        {
            Throws.ThrowNativeValueNull();
        }

        SetHandle(value.Value);
        Ort.OrtTensorTypeAndShapeInfo* tensorInfo;
        try
        {
            Ort.GetTensorTypeAndShape(value, &tensorInfo).Ok();
        }
        catch
        {
            Dispose();
            throw;
        }

        try
        {
            Ort.ONNXTensorElementDataType elementType;
            Ort.GetTensorElementType(tensorInfo, &elementType).Ok();
            ElementType = elementType;
            nuint dimensionCount;
            Ort.GetDimensionsCount(tensorInfo, &dimensionCount).Ok();
            _dimensions = new long[checked((int)dimensionCount)];
            fixed (long* dimensionsPointer = _dimensions)
            {
                Ort.GetDimensions(tensorInfo, dimensionsPointer, dimensionCount).Ok();
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
        ThrowIfDisposed();
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
        Ort.GetTensorMutableData(Handle, &data).Ok();
        return new Span<T>(data, checked((int)elementCount));
    }

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseValue(Handle);
        return true;
    }
}
