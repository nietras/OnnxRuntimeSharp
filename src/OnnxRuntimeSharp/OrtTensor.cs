using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtTensor<T> : OrtSafeHandle<Ort.OrtValueHandle> where T : unmanaged
{
    readonly GCHandle _dataHandle;
    readonly OrtMemoryInfo? _memoryInfo;
    readonly bool _memoryInfoReferenceAdded;

    public OrtTensor(T[] data, ReadOnlySpan<long> dimensions)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Length == 0)
        {
            Throws.ThrowTensorDataEmpty();
        }

        var elementCount = GetElementCount(dimensions);
        if (elementCount != data.Length)
        {
            Throws.ThrowTensorDimensionsDataLengthMismatch();
        }

        _dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
        Ort.OrtMemoryInfoHandle memoryInfo = default;
        try
        {
            Ort.Ok(Ort.CreateCpuMemoryInfo(Ort.OrtAllocatorType.OrtArenaAllocator, Ort.OrtMemType.OrtMemTypeDefault, &memoryInfo));
            fixed (long* dimensionsPointer = dimensions)
            {
                Ort.OrtValueHandle value;
                Ort.Ok(Ort.CreateTensorWithDataAsOrtValue(
                    memoryInfo,
                    _dataHandle.AddrOfPinnedObject().ToPointer(),
                    checked((nuint)(data.Length * sizeof(T))),
                    dimensionsPointer,
                    (nuint)dimensions.Length,
                    OrtTensorElementType.Get<T>(),
                    &value));
                SetHandle(value.Value);
            }
        }
        catch
        {
            _dataHandle.Free();
            throw;
        }
        finally
        {
            if (!memoryInfo.IsNull)
            {
                Ort.ReleaseMemoryInfo(memoryInfo);
            }
        }
    }

    public OrtTensor(
        T* data,
        int elementCount,
        ReadOnlySpan<long> dimensions,
        OrtMemoryInfo memoryInfo)
    {
        if (data is null)
        {
            Throws.ThrowNativeTensorDataNull();
        }
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(elementCount);
        ArgumentNullException.ThrowIfNull(memoryInfo);
        if (GetElementCount(dimensions) != elementCount)
        {
            Throws.ThrowTensorDimensionsElementCountMismatch();
        }

        var memoryInfoReferenceAdded = false;
        try
        {
            memoryInfo.DangerousAddRef(ref memoryInfoReferenceAdded);
            fixed (long* dimensionsPointer = dimensions)
            {
                Ort.OrtValueHandle value;
                Ort.Ok(Ort.CreateTensorWithDataAsOrtValue(
                    memoryInfo.Handle,
                    data,
                    checked((nuint)(elementCount * sizeof(T))),
                    dimensionsPointer,
                    (nuint)dimensions.Length,
                    OrtTensorElementType.Get<T>(),
                    &value));
                SetHandle(value.Value);
            }
            _memoryInfo = memoryInfo;
            _memoryInfoReferenceAdded = memoryInfoReferenceAdded;
            memoryInfoReferenceAdded = false;
        }
        finally
        {
            if (memoryInfoReferenceAdded)
            {
                memoryInfo.DangerousRelease();
            }
        }
    }

    public Span<T> Data
    {
        get
        {
            ThrowIfDisposed();
            if (!_dataHandle.IsAllocated)
            {
                Throws.ThrowExternallyOwnedTensorData();
            }
            return ((T[])_dataHandle.Target!).AsSpan();
        }
    }

    public Ort.ONNXTensorElementDataType ElementType => OrtTensorElementType.Get<T>();

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseValue(Handle);
        if (_dataHandle.IsAllocated)
        {
            _dataHandle.Free();
        }
        if (_memoryInfoReferenceAdded)
        {
            _memoryInfo!.DangerousRelease();
        }
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
                Throws.ThrowNegativeTensorDimension();
            }

            checked
            {
                count *= dimension;
            }
        }
        return checked((int)count);
    }
}
