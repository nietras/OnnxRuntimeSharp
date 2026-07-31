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
            throw new ArgumentNullException(nameof(data));
        }
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(elementCount);
        ArgumentNullException.ThrowIfNull(memoryInfo);
        if (GetElementCount(dimensions) != elementCount)
        {
            throw new ArgumentException("Tensor dimensions do not match element count.", nameof(dimensions));
        }

        var memoryInfoReferenceAdded = false;
        try
        {
            memoryInfo.DangerousAddRef(ref memoryInfoReferenceAdded);
            fixed (long* dimensionsPointer = dimensions)
            {
                Ort.OrtValue* value;
                Ort.ThrowIfError(Ort.CreateTensorWithDataAsOrtValue(
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
                throw new InvalidOperationException("The tensor wraps externally owned native memory.");
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
                throw new ArgumentOutOfRangeException(nameof(dimensions), "Tensor dimensions must be non-negative.");
            }

            checked
            {
                count *= dimension;
            }
        }

        return checked((int)count);
    }

}
