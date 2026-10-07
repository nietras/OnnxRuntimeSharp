using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtRunOptions : OrtSafeHandle<Ort.OrtRunOptionsHandle>
{
    public OrtRunOptions()
    {
        Ort.OrtRunOptionsHandle options;
        Ort.Ok(Ort.CreateRunOptions(&options));
        SetHandle(options.Value);
    }

    public int GetLogVerbosityLevel()
    {
        ThrowIfDisposed();
        int value;
        Ort.Ok(Ort.RunOptionsGetRunLogVerbosityLevel(Handle, &value));
        return value;
    }

    public void SetLogVerbosityLevel(int level)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegative(level);
        Ort.Ok(Ort.RunOptionsSetRunLogVerbosityLevel(Handle, level));
    }

    public Ort.OrtLoggingLevel GetLogSeverityLevel()
    {
        ThrowIfDisposed();
        int value;
        Ort.Ok(Ort.RunOptionsGetRunLogSeverityLevel(Handle, &value));
        return (Ort.OrtLoggingLevel)value;
    }

    public void SetLogSeverityLevel(Ort.OrtLoggingLevel level)
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.RunOptionsSetRunLogSeverityLevel(Handle, (int)level));
    }

    public string GetTag()
    {
        ThrowIfDisposed();
        sbyte* value;
        Ort.Ok(Ort.RunOptionsGetRunTag(Handle, &value));
        return Marshal.PtrToStringUTF8((IntPtr)value) ?? string.Empty;
    }

    public void SetTag(string tag)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(tag);
        var utf8Value = Utf8StringMarshaller.ConvertToUnmanaged(tag);
        try
        {
            Ort.Ok(Ort.RunOptionsSetRunTag(Handle, (sbyte*)utf8Value));
        }
        finally
        {
            Utf8StringMarshaller.Free(utf8Value);
        }
    }

    public void AddConfigEntry(string key, string value)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);
        var utf8Key = Utf8StringMarshaller.ConvertToUnmanaged(key);
        var utf8Value = Utf8StringMarshaller.ConvertToUnmanaged(value);
        try
        {
            Ort.Ok(Ort.AddRunConfigEntry(Handle, (sbyte*)utf8Key, (sbyte*)utf8Value));
        }
        finally
        {
            Utf8StringMarshaller.Free(utf8Value);
            Utf8StringMarshaller.Free(utf8Key);
        }
    }

    public void RequestTermination()
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.RunOptionsSetTerminate(Handle));
    }

    public void ResetTermination()
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.RunOptionsUnsetTerminate(Handle));
    }

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseRunOptions(Handle);
        return true;
    }
}
