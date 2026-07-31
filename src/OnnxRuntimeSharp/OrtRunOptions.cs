using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtRunOptions : SafeHandle
{
    public OrtRunOptions()
        : base(IntPtr.Zero, ownsHandle: true)
    {
        Ort.OrtRunOptions* options;
        Ort.ThrowIfError(Ort.CreateRunOptions(&options));
        SetHandle((IntPtr)options);
    }

    public int LogVerbosityLevel
    {
        get
        {
            ThrowIfDisposed();
            int value;
            Ort.ThrowIfError(Ort.RunOptionsGetRunLogVerbosityLevel(Pointer, &value));
            return value;
        }
        set
        {
            ThrowIfDisposed();
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            Ort.ThrowIfError(Ort.RunOptionsSetRunLogVerbosityLevel(Pointer, value));
        }
    }

    public Ort.OrtLoggingLevel LogSeverityLevel
    {
        get
        {
            ThrowIfDisposed();
            int value;
            Ort.ThrowIfError(Ort.RunOptionsGetRunLogSeverityLevel(Pointer, &value));
            return (Ort.OrtLoggingLevel)value;
        }
        set
        {
            ThrowIfDisposed();
            Ort.ThrowIfError(Ort.RunOptionsSetRunLogSeverityLevel(Pointer, (int)value));
        }
    }

    public string Tag
    {
        get
        {
            ThrowIfDisposed();
            sbyte* value;
            Ort.ThrowIfError(Ort.RunOptionsGetRunTag(Pointer, &value));
            return Marshal.PtrToStringUTF8((IntPtr)value) ?? string.Empty;
        }
        set
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(value);
            var utf8Value = Utf8StringMarshaller.ConvertToUnmanaged(value);
            try
            {
                Ort.ThrowIfError(Ort.RunOptionsSetRunTag(Pointer, (sbyte*)utf8Value));
            }
            finally
            {
                Utf8StringMarshaller.Free(utf8Value);
            }
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
            Ort.ThrowIfError(Ort.AddRunConfigEntry(Pointer, (sbyte*)utf8Key, (sbyte*)utf8Value));
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
        Ort.ThrowIfError(Ort.RunOptionsSetTerminate(Pointer));
    }

    public void ResetTermination()
    {
        ThrowIfDisposed();
        Ort.ThrowIfError(Ort.RunOptionsUnsetTerminate(Pointer));
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    internal Ort.OrtRunOptions* Pointer => (Ort.OrtRunOptions*)handle;

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseRunOptions(Pointer);
        return true;
    }

    void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(IsClosed || IsInvalid, this);
}
