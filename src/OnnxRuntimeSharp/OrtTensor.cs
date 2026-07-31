using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtTensor<T> : SafeHandle where T : unmanaged
{
    readonly GCHandle _dataHandle;

    public OrtTensor(T[] data, ReadOnlySpan<long> dimensions)
        : base(IntPtr.Zero, ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Length == 0)
        {
            throw new ArgumentException("Tensor data cannot be empty.", nameof(data));
        }

        var elementCount = GetElementCount(dimensions);
        if (elementCount != data.Length)
        {
            throw new ArgumentException("Tensor dimensions do not match data length.", nameof(dimensions));
        }

        _dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
        Ort.OrtMemoryInfo* memoryInfo = null;
        try
        {
            Ort.ThrowIfError(Ort.CreateCpuMemoryInfo(Ort.OrtAllocatorType.OrtArenaAllocator, Ort.OrtMemType.OrtMemTypeDefault, &memoryInfo));
            fixed (long* dimensionsPointer = dimensions)
            {
                Ort.OrtValue* value;
                Ort.ThrowIfError(Ort.CreateTensorWithDataAsOrtValue(
                    memoryInfo,
                    _dataHandle.AddrOfPinnedObject().ToPointer(),
                    checked((nuint)(data.Length * sizeof(T))),
                    dimensionsPointer,
                    (nuint)dimensions.Length,
                    GetElementType(),
                    &value));
                SetHandle((IntPtr)value);
            }
        }
        catch
        {
            _dataHandle.Free();
            throw;
        }
        finally
        {
            if (memoryInfo is not null)
            {
                Ort.ReleaseMemoryInfo(memoryInfo);
            }
        }
    }

    public Span<T> Data => ((T[])_dataHandle.Target!).AsSpan();

    public Ort.ONNXTensorElementDataType ElementType => GetElementType();

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseValue((Ort.OrtValue*)handle);
        _dataHandle.Free();
        return true;
    }

    static int GetElementCount(ReadOnlySpan<long> dimensions)
    {
        if (dimensions.IsEmpty)
        {
            return 1;
        }

        long count = 1;
        foreach (var dimension in dimensions)
        {
            if (dimension < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dimensions), "Tensor dimensions must be non-negative.");
            }

            checked
            {
                count *= dimension;
            }
        }

        return checked((int)count);
    }

    static Ort.ONNXTensorElementDataType GetElementType() => typeof(T) == typeof(float) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT
        : typeof(T) == typeof(byte) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT8
        : typeof(T) == typeof(sbyte) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_INT8
        : typeof(T) == typeof(ushort) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT16
        : typeof(T) == typeof(short) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_INT16
        : typeof(T) == typeof(int) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_INT32
        : typeof(T) == typeof(long) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_INT64
        : typeof(T) == typeof(bool) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_BOOL
        : typeof(T) == typeof(Half) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT16
        : typeof(T) == typeof(double) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_DOUBLE
        : throw new NotSupportedException($"ONNX Runtime does not support {typeof(T)} tensor interop.");
}
