using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtEnv : OrtSafeHandle<Ort.OrtEnvHandle>
{
    public OrtEnv(string logId = "OnnxRuntimeSharp",
                  Ort.OrtLoggingLevel loggingLevel = Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_WARNING)
    {
        var ortEnv = Ort.CreateEnvironment(logId, loggingLevel);
        SetHandle(ortEnv.Value);
    }

    public IReadOnlyList<OrtEpDevice> GetExecutionProviderDevices()
    {
        ThrowIfDisposed();
        var referenceAdded = false;
        try
        {
            DangerousAddRef(ref referenceAdded);
            Ort.OrtEpDevice** devices;
            nuint deviceCount;
            Ort.GetEpDevices(Handle, &devices, &deviceCount).Ok();
            var result = new OrtEpDevice[checked((int)deviceCount)];
            for (var index = 0; index < result.Length; ++index)
            {
                result[index] = new OrtEpDevice(this, devices[index]);
            }
            return result;
        }
        finally
        {
            if (referenceAdded)
            {
                DangerousRelease();
            }
        }
    }

    public void RegisterExecutionProviderLibrary(string registrationName, string libraryPath)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(registrationName);
        ArgumentException.ThrowIfNullOrWhiteSpace(libraryPath);
        var utf8Name = Utf8StringMarshaller.ConvertToUnmanaged(registrationName);
        var referenceAdded = false;
        try
        {
            DangerousAddRef(ref referenceAdded);
            if (OperatingSystem.IsWindows())
            {
                fixed (char* pathPointer = libraryPath)
                {
                    Ort.Ok(Ort.RegisterExecutionProviderLibrary(
                        Handle,
                        (sbyte*)utf8Name,
                        (ushort*)pathPointer));
                }
            }
            else
            {
                var utf8Path = Utf8StringMarshaller.ConvertToUnmanaged(libraryPath);
                try
                {
                    Ort.Ok(Ort.RegisterExecutionProviderLibrary(
                        Handle,
                        (sbyte*)utf8Name,
                        (ushort*)utf8Path));
                }
                finally
                {
                    Utf8StringMarshaller.Free(utf8Path);
                }
            }
        }
        finally
        {
            if (referenceAdded)
            {
                DangerousRelease();
            }
            Utf8StringMarshaller.Free(utf8Name);
        }
    }

    public void UnregisterExecutionProviderLibrary(string registrationName)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(registrationName);
        var utf8Name = Utf8StringMarshaller.ConvertToUnmanaged(registrationName);
        var referenceAdded = false;
        try
        {
            DangerousAddRef(ref referenceAdded);
            Ort.Ok(Ort.UnregisterExecutionProviderLibrary(Handle, (sbyte*)utf8Name));
        }
        finally
        {
            if (referenceAdded)
            {
                DangerousRelease();
            }
            Utf8StringMarshaller.Free(utf8Name);
        }
    }

    public void SetLogLevel(Ort.OrtLoggingLevel loggingLevel)
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.UpdateEnvWithCustomLogLevel(Handle, loggingLevel));
    }

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseEnv(Handle);
        return true;
    }

    void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(IsClosed || IsInvalid, this);
}
