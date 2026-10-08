using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtEnv : OrtSafeHandle<Ort.OrtEnvHandle>
{
    static readonly Lazy<OrtEnv> _instance = new(() => new OrtEnv());

    /// <summary>Returns the lazily created, shared environment with default settings.</summary>
    /// <remarks>Do not dispose the shared instance. Use the constructor for an independently owned environment.</remarks>
    public static OrtEnv Instance()
    {
        return _instance.Value;
    }

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
            fixed (char* pathPointer = libraryPath)
            {
                using var nativePath = new OrtNativePath(libraryPath, pathPointer);
                Ort.RegisterExecutionProviderLibrary(
                    Handle,
                    (sbyte*)utf8Name,
                    nativePath.Pointer).Ok();
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
            Ort.UnregisterExecutionProviderLibrary(Handle, (sbyte*)utf8Name).Ok();
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
        Ort.UpdateEnvWithCustomLogLevel(Handle, loggingLevel).Ok();
    }

    protected override bool ReleaseHandle()
    {
        Ort.ReleaseEnv(Handle);
        return true;
    }
}
