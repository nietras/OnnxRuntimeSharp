using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtValue<T> : OrtValue where T : unmanaged
{
    readonly GCHandle _dataHandle;
    readonly OrtMemoryInfo? _memoryInfo;
    readonly bool _memoryInfoReferenceAdded;

    public OrtValue(T[] data, ReadOnlySpan<long> dimensions)
        : base(ValidateManagedData(data, dimensions), dimensions, OrtTensorElementType.Get<T>())
    {
        _dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
        Ort.OrtMemoryInfoHandle memoryInfo = default;
        try
        {
            Ort.CreateCpuMemoryInfo(Ort.OrtAllocatorType.OrtArenaAllocator, Ort.OrtMemType.OrtMemTypeDefault, &memoryInfo).Ok();
            fixed (long* dimensionsPointer = dimensions)
            {
                Ort.OrtValueHandle value;
                Ort.CreateTensorWithDataAsOrtValue(
                    memoryInfo,
                    _dataHandle.AddrOfPinnedObject().ToPointer(),
                    checked((nuint)data.Length * (nuint)sizeof(T)),
                    dimensionsPointer,
                    (nuint)dimensions.Length,
                    ElementType,
                    &value).Ok();
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

    public OrtValue(
        T* data,
        int elementCount,
        ReadOnlySpan<long> dimensions,
        OrtMemoryInfo memoryInfo)
        : base(ValidateNativeData(data, elementCount, dimensions, memoryInfo), dimensions, OrtTensorElementType.Get<T>())
    {
        var memoryInfoReferenceAdded = false;
        try
        {
            memoryInfo.DangerousAddRef(ref memoryInfoReferenceAdded);
            fixed (long* dimensionsPointer = dimensions)
            {
                Ort.OrtValueHandle value;
                Ort.CreateTensorWithDataAsOrtValue(
                    memoryInfo.Handle,
                    data,
                    checked((nuint)elementCount * (nuint)sizeof(T)),
                    dimensionsPointer,
                    (nuint)dimensions.Length,
                    ElementType,
                    &value).Ok();
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

    public Span<T> GetTensorData() => base.GetTensorData<T>();

    protected override bool ReleaseHandle()
    {
        var released = base.ReleaseHandle();
        if (_dataHandle.IsAllocated)
        {
            _dataHandle.Free();
        }
        if (_memoryInfoReferenceAdded)
        {
            _memoryInfo!.DangerousRelease();
        }
        return released;
    }

    static int ValidateManagedData(T[] data, ReadOnlySpan<long> dimensions)
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
        return elementCount;
    }

    static int ValidateNativeData(T* data, int elementCount, ReadOnlySpan<long> dimensions, OrtMemoryInfo memoryInfo)
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
        return elementCount;
    }
}
