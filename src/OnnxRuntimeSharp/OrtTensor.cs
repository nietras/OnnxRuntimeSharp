using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtTensor<T> : SafeHandle where T : unmanaged
{
    readonly GCHandle _dataHandle;
    readonly OrtMemoryInfo? _memoryInfo;
    readonly bool _memoryInfoReferenceAdded;

    public OrtTensor(T[] data, ReadOnlySpan<long> dimensions)
        : base(IntPtr.Zero, ownsHandle: true)
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
        Ort.OrtMemoryInfo* memoryInfo = null;
        try
        {
            Ort.Ok(Ort.CreateCpuMemoryInfo(Ort.OrtAllocatorType.OrtArenaAllocator, Ort.OrtMemType.OrtMemTypeDefault, &memoryInfo));
            fixed (long* dimensionsPointer = dimensions)
            {
                Ort.OrtValue* value;
                Ort.Ok(Ort.CreateTensorWithDataAsOrtValue(
                    memoryInfo,
                    _dataHandle.AddrOfPinnedObject().ToPointer(),
                    checked((nuint)(data.Length * sizeof(T))),
                    dimensionsPointer,
                    (nuint)dimensions.Length,
                    OrtTensorElementType.Get<T>(),
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

    public OrtTensor(
        T* data,
        int elementCount,
        ReadOnlySpan<long> dimensions,
        OrtMemoryInfo memoryInfo)
        : base(IntPtr.Zero, ownsHandle: true)
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
                Ort.OrtValue* value;
                Ort.Ok(Ort.CreateTensorWithDataAsOrtValue(
                    memoryInfo.Pointer,
                    data,
                    checked((nuint)(elementCount * sizeof(T))),
                    dimensionsPointer,
                    (nuint)dimensions.Length,
                    OrtTensorElementType.Get<T>(),
                    &value));
                SetHandle((IntPtr)value);
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
            ObjectDisposedException.ThrowIf(IsClosed || IsInvalid, this);
            if (!_dataHandle.IsAllocated)
            {
                Throws.ThrowExternallyOwnedTensorData();
            }
            return ((T[])_dataHandle.Target!).AsSpan();
        }
    }

    public Ort.ONNXTensorElementDataType ElementType => OrtTensorElementType.Get<T>();

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseValue((Ort.OrtValue*)handle);
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
