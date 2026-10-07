using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtRunOptions : OrtSafeHandle<Ort.OrtRunOptionsHandle>
{
    public OrtRunOptions()
    {
        Ort.OrtRunOptionsHandle options;
        Ort.CreateRunOptions(&options).Ok();
        SetHandle(options.Value);
    }

    public int GetLogVerbosityLevel()
    {
        ThrowIfDisposed();
        int value;
        Ort.RunOptionsGetRunLogVerbosityLevel(Handle, &value).Ok();
        return value;
    }

    public void SetLogVerbosityLevel(int level)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegative(level);
        Ort.RunOptionsSetRunLogVerbosityLevel(Handle, level).Ok();
    }

    public Ort.OrtLoggingLevel GetLogSeverityLevel()
    {
        ThrowIfDisposed();
        int value;
        Ort.RunOptionsGetRunLogSeverityLevel(Handle, &value).Ok();
        return (Ort.OrtLoggingLevel)value;
    }

    public void SetLogSeverityLevel(Ort.OrtLoggingLevel level)
    {
        ThrowIfDisposed();
        Ort.RunOptionsSetRunLogSeverityLevel(Handle, (int)level).Ok();
    }

    public string GetTag()
    {
        ThrowIfDisposed();
        sbyte* value;
        Ort.RunOptionsGetRunTag(Handle, &value).Ok();
        return Marshal.PtrToStringUTF8((IntPtr)value) ?? string.Empty;
    }

    public void SetTag(string tag)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(tag);
        var utf8Value = Utf8StringMarshaller.ConvertToUnmanaged(tag);
        try
        {
            Ort.RunOptionsSetRunTag(Handle, (sbyte*)utf8Value).Ok();
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
            Ort.AddRunConfigEntry(Handle, (sbyte*)utf8Key, (sbyte*)utf8Value).Ok();
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
        Ort.RunOptionsSetTerminate(Handle).Ok();
    }

    public void ResetTermination()
    {
        ThrowIfDisposed();
        Ort.RunOptionsUnsetTerminate(Handle).Ok();
    }

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseRunOptions(Handle);
        return true;
    }
}
