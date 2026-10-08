using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtMemoryInfo : OrtSafeHandle<Ort.OrtMemoryInfoHandle>
{
    public OrtMemoryInfo(
        string allocatorName,
        Ort.OrtAllocatorType allocatorType,
        int deviceId,
        Ort.OrtMemType memoryType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(allocatorName);
        var utf8Name = Utf8StringMarshaller.ConvertToUnmanaged(allocatorName);
        try
        {
            Ort.OrtMemoryInfoHandle info;
            Ort.CreateMemoryInfo(
                (sbyte*)utf8Name,
                allocatorType,
                deviceId,
                memoryType,
                &info).Ok();
            SetHandle(info.Value);
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
        Ort.OrtMemoryInfoHandle info;
        Ort.CreateCpuMemoryInfo(allocatorType, memoryType, &info).Ok();
        return new OrtMemoryInfo(info);
    }

    OrtMemoryInfo(Ort.OrtMemoryInfoHandle info) => SetHandle(info.Value);

    public string GetName()
    {
        ThrowIfDisposed();
        sbyte* value;
        Ort.MemoryInfoGetName(Handle, &value).Ok();
        return Marshal.PtrToStringUTF8((IntPtr)value) ?? string.Empty;
    }

    public int GetDeviceId()
    {
        ThrowIfDisposed();
        int value;
        Ort.MemoryInfoGetId(Handle, &value).Ok();
        return value;
    }

    public Ort.OrtMemType GetMemoryType()
    {
        ThrowIfDisposed();
        Ort.OrtMemType value;
        Ort.MemoryInfoGetMemType(Handle, &value).Ok();
        return value;
    }

    public Ort.OrtAllocatorType GetAllocatorType()
    {
        ThrowIfDisposed();
        Ort.OrtAllocatorType value;
        Ort.MemoryInfoGetType(Handle, &value).Ok();
        return value;
    }

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseMemoryInfo(Handle);
        return true;
    }
}
