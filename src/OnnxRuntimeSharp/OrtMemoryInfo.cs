using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtMemoryInfo : SafeHandle
{
    public OrtMemoryInfo(
        string allocatorName,
        Ort.OrtAllocatorType allocatorType,
        int deviceId,
        Ort.OrtMemType memoryType)
        : base(IntPtr.Zero, ownsHandle: true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(allocatorName);
        var utf8Name = Utf8StringMarshaller.ConvertToUnmanaged(allocatorName);
        try
        {
            Ort.OrtMemoryInfo* info;
            Ort.Ok(Ort.CreateMemoryInfo(
                (sbyte*)utf8Name,
                allocatorType,
                deviceId,
                memoryType,
                &info));
            SetHandle((IntPtr)info);
        }
        finally
        {
            Utf8StringMarshaller.Free(utf8Name);
        }
    }

    public static OrtMemoryInfo CreateCpu(
        Ort.OrtAllocatorType allocatorType = Ort.OrtAllocatorType.OrtArenaAllocator,
        Ort.OrtMemType memoryType = Ort.OrtMemType.OrtMemTypeDefault)
    {
        Ort.OrtMemoryInfo* info;
        Ort.Ok(Ort.CreateCpuMemoryInfo(allocatorType, memoryType, &info));
        return new OrtMemoryInfo(info);
    }

    OrtMemoryInfo(Ort.OrtMemoryInfo* info)
        : base(IntPtr.Zero, ownsHandle: true) =>
        SetHandle((IntPtr)info);

    public string Name
    {
        get
        {
            ThrowIfDisposed();
            sbyte* value;
            Ort.Ok(Ort.MemoryInfoGetName(Pointer, &value));
            return Marshal.PtrToStringUTF8((IntPtr)value) ?? string.Empty;
        }
    }

    public int DeviceId
    {
        get
        {
            ThrowIfDisposed();
            int value;
            Ort.Ok(Ort.MemoryInfoGetId(Pointer, &value));
            return value;
        }
    }

    public Ort.OrtMemType MemoryType
    {
        get
        {
            ThrowIfDisposed();
            Ort.OrtMemType value;
            Ort.Ok(Ort.MemoryInfoGetMemType(Pointer, &value));
            return value;
        }
    }

    public Ort.OrtAllocatorType AllocatorType
    {
        get
        {
            ThrowIfDisposed();
            Ort.OrtAllocatorType value;
            Ort.Ok(Ort.MemoryInfoGetType(Pointer, &value));
            return value;
        }
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    internal Ort.OrtMemoryInfo* Pointer => (Ort.OrtMemoryInfo*)handle;

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseMemoryInfo(Pointer);
        return true;
    }

    void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(IsClosed || IsInvalid, this);
}
