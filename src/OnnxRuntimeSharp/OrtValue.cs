using System;

namespace OnnxRuntimeSharp;

public unsafe class OrtValue : OrtSafeHandle<Ort.OrtValueHandle>
{
    readonly long[] _dimensions;

    private protected OrtValue(
        int elementCount,
        ReadOnlySpan<long> dimensions,
        Ort.ONNXTensorElementDataType elementType)
    {
        ElementCount = elementCount;
        _dimensions = dimensions.ToArray();
        ElementType = elementType;
    }

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
            ElementCount = GetElementCount(_dimensions);
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

    private protected int ElementCount { get; }

    public Span<T> GetTensorData<T>() where T : unmanaged
    {
        ThrowIfDisposed();
        var expectedType = OrtTensorElementType.Get<T>();
        if (ElementType != expectedType)
        {
            Throws.ThrowTensorElementTypeMismatch(ElementType, expectedType);
        }

        Ort.OrtMemoryInfoHandle memoryInfo;
        Ort.GetTensorMemoryInfo(Handle, &memoryInfo).Ok();
        Ort.OrtMemoryInfoDeviceType deviceType;
        Ort.MemoryInfoGetDeviceType(memoryInfo, &deviceType);
        if (deviceType != Ort.OrtMemoryInfoDeviceType.OrtMemoryInfoDeviceType_CPU)
        {
            Throws.ThrowTensorDataNotCpuAccessible();
        }

        void* data;
        Ort.GetTensorMutableData(Handle, &data).Ok();
        return new Span<T>(data, ElementCount);
    }

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseValue(Handle);
        return true;
    }

    private protected static int GetElementCount(ReadOnlySpan<long> dimensions)
    {
        long count = 1;
        foreach (var dimension in dimensions)
        {
            if (dimension < 0)
            {
                Throws.ThrowNegativeTensorDimension();
            }
            count = checked(count * dimension);
        }
        return checked((int)count);
    }
}
