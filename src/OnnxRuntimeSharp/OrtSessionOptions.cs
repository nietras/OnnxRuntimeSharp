using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

public sealed class OrtSessionOptions : SafeHandle
{
    public OrtSessionOptions()
        : base(IntPtr.Zero, ownsHandle: true)
    {
        unsafe
        {
            Ort.OrtSessionOptions* options;
            Ort.Ok(Ort.CreateSessionOptions(&options));
            SetHandle((IntPtr)options);
            Ort.Ok(Ort.SetSessionGraphOptimizationLevel(
                options,
                Ort.GraphOptimizationLevel.ORT_ENABLE_ALL));
        }
    }

    public unsafe void EnableProfiling(string profileFilePrefix)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(profileFilePrefix);
        if (OperatingSystem.IsWindows())
        {
            fixed (char* utf16Prefix = profileFilePrefix)
            {
                Ort.Ok(Ort.EnableProfiling((Ort.OrtSessionOptions*)handle, (ushort*)utf16Prefix));
            }

            return;
        }

        var utf8Prefix = Utf8StringMarshaller.ConvertToUnmanaged(profileFilePrefix);
        try
        {
            Ort.Ok(Ort.EnableProfiling((Ort.OrtSessionOptions*)handle, (ushort*)utf8Prefix));
        }
        finally
        {
            Utf8StringMarshaller.Free(utf8Prefix);
        }
    }

    public unsafe void DisableProfiling()
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.DisableProfiling(Pointer));
    }

    public unsafe void SetIntraOpThreadCount(int threadCount)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(threadCount);
        Ort.Ok(Ort.SetIntraOpNumThreads(Pointer, threadCount));
    }

    public unsafe void SetInterOpThreadCount(int threadCount)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(threadCount);
        Ort.Ok(Ort.SetInterOpNumThreads(Pointer, threadCount));
    }

    public unsafe void SetGraphOptimizationLevel(Ort.GraphOptimizationLevel graphOptimizationLevel)
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SetSessionGraphOptimizationLevel(Pointer, graphOptimizationLevel));
    }

    public unsafe void SetExecutionMode(Ort.ExecutionMode executionMode)
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SetSessionExecutionMode(Pointer, executionMode));
    }

    public unsafe void SetMemoryPatternEnabled(bool enabled)
    {
        ThrowIfDisposed();
        Ort.Ok(enabled ? Ort.EnableMemPattern(Pointer) : Ort.DisableMemPattern(Pointer));
    }

    public unsafe void SetCpuMemoryArenaEnabled(bool enabled)
    {
        ThrowIfDisposed();
        Ort.Ok(enabled ? Ort.EnableCpuMemArena(Pointer) : Ort.DisableCpuMemArena(Pointer));
    }

    public unsafe void SetDeterministicCompute(bool enabled)
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SetDeterministicCompute(Pointer, enabled ? (byte)1 : (byte)0));
    }

    public unsafe void DisablePerSessionThreads()
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.DisablePerSessionThreads(Pointer));
    }

    public unsafe void SetLogVerbosityLevel(int level)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegative(level);
        Ort.Ok(Ort.SetSessionLogVerbosityLevel(Pointer, level));
    }

    public unsafe void SetLogSeverityLevel(Ort.OrtLoggingLevel level)
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SetSessionLogSeverityLevel(Pointer, (int)level));
    }

    public unsafe void SetLogId(string logId)
    {
        ThrowIfDisposed();
        InvokeUtf8(logId, static (options, value) => Ort.SetSessionLogId(options, value));
    }

    public unsafe void AddFreeDimensionOverride(string denotation, long value)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        InvokeUtf8(denotation, (options, name) => Ort.AddFreeDimensionOverride(options, name, value));
    }

    public unsafe void AddFreeDimensionOverrideByName(string name, long value)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        InvokeUtf8(name, (options, dimensionName) => Ort.AddFreeDimensionOverrideByName(options, dimensionName, value));
    }

    public unsafe void AddConfigEntry(string key, string value)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);
        var utf8Key = Utf8StringMarshaller.ConvertToUnmanaged(key);
        var utf8Value = Utf8StringMarshaller.ConvertToUnmanaged(value);
        try
        {
            Ort.Ok(Ort.AddSessionConfigEntry(Pointer, (sbyte*)utf8Key, (sbyte*)utf8Value));
        }
        finally
        {
            Utf8StringMarshaller.Free(utf8Value);
            Utf8StringMarshaller.Free(utf8Key);
        }
    }

    public unsafe void SetOptimizedModelFilePath(string path)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (OperatingSystem.IsWindows())
        {
            fixed (char* pathPointer = path)
            {
                Ort.Ok(Ort.SetOptimizedModelFilePath(Pointer, (ushort*)pathPointer));
            }
            return;
        }

        var utf8Path = Utf8StringMarshaller.ConvertToUnmanaged(path);
        try
        {
            Ort.Ok(Ort.SetOptimizedModelFilePath(Pointer, (ushort*)utf8Path));
        }
        finally
        {
            Utf8StringMarshaller.Free(utf8Path);
        }
    }

    public unsafe void AppendExecutionProvider(string providerName)
        => AppendExecutionProvider(providerName, null);

    public unsafe void AppendExecutionProvider(
        string providerName,
        IReadOnlyDictionary<string, string>? providerOptions)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);
        if (string.Equals(providerName, "CUDAExecutionProvider", StringComparison.Ordinal))
        {
            AppendCudaExecutionProvider(providerOptions);
            return;
        }
        if (string.Equals(providerName, "TensorrtExecutionProvider", StringComparison.Ordinal))
        {
            AppendTensorRtExecutionProvider(providerOptions);
            return;
        }

        var utf8ProviderName = Utf8StringMarshaller.ConvertToUnmanaged(providerName);
        var optionCount = providerOptions?.Count ?? 0;
        var keys = stackalloc sbyte*[optionCount];
        var values = stackalloc sbyte*[optionCount];
        var initializedCount = 0;
        try
        {
            if (providerOptions is not null)
            {
                foreach (var option in providerOptions)
                {
                    keys[initializedCount] = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(option.Key);
                    values[initializedCount] = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(option.Value);
                    ++initializedCount;
                }
            }

            Ort.Ok(Ort.SessionOptionsAppendExecutionProvider(
                Pointer,
                (sbyte*)utf8ProviderName,
                keys,
                values,
                (nuint)optionCount));
        }
        finally
        {
            for (var index = 0; index < initializedCount; ++index)
            {
                Utf8StringMarshaller.Free((byte*)values[index]);
                Utf8StringMarshaller.Free((byte*)keys[index]);
            }
            Utf8StringMarshaller.Free(utf8ProviderName);
        }
    }

    public unsafe void AppendExecutionProvider(
        OrtEnvironment environment,
        ReadOnlySpan<OrtEpDevice> devices,
        IReadOnlyDictionary<string, string>? providerOptions = null)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(environment);
        if (devices.IsEmpty)
        {
            Throws.ThrowExecutionProviderDevicesEmpty();
        }

        var nativeDevices = stackalloc Ort.OrtEpDevice*[devices.Length];
        for (var index = 0; index < devices.Length; ++index)
        {
            ArgumentNullException.ThrowIfNull(devices[index], nameof(devices));
            if (!ReferenceEquals(devices[index].Environment, environment))
            {
                Throws.ThrowExecutionProviderDeviceEnvironmentMismatch();
            }
            if (index > 0 &&
                !string.Equals(
                    devices[0].ExecutionProviderName,
                    devices[index].ExecutionProviderName,
                    StringComparison.Ordinal))
            {
                Throws.ThrowExecutionProviderDeviceNameMismatch();
            }
            nativeDevices[index] = devices[index].Pointer;
        }

        var optionCount = providerOptions?.Count ?? 0;
        var keys = stackalloc sbyte*[optionCount];
        var values = stackalloc sbyte*[optionCount];
        var initializedCount = 0;
        var environmentReferenceAdded = false;
        try
        {
            environment.DangerousAddRef(ref environmentReferenceAdded);
            if (providerOptions is not null)
            {
                foreach (var option in providerOptions)
                {
                    keys[initializedCount] = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(option.Key);
                    values[initializedCount] = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(option.Value);
                    ++initializedCount;
                }
            }
            Ort.Ok(Ort.SessionOptionsAppendExecutionProvider_V2(
                Pointer,
                environment.Pointer,
                nativeDevices,
                (nuint)devices.Length,
                keys,
                values,
                (nuint)optionCount));
        }
        finally
        {
            for (var index = 0; index < initializedCount; ++index)
            {
                Utf8StringMarshaller.Free((byte*)values[index]);
                Utf8StringMarshaller.Free((byte*)keys[index]);
            }
            if (environmentReferenceAdded)
            {
                environment.DangerousRelease();
            }
        }
    }

    public unsafe void SetExecutionProviderSelectionPolicy(Ort.OrtExecutionProviderDevicePolicy policy)
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SessionOptionsSetEpSelectionPolicy(Pointer, policy));
    }

    public unsafe void AppendCudaExecutionProvider(IReadOnlyDictionary<string, string>? providerOptions = null)
    {
        ThrowIfDisposed();
        Ort.OrtCUDAProviderOptionsV2* nativeProviderOptions;
        Ort.Ok(Ort.CreateCUDAProviderOptions(&nativeProviderOptions));
        try
        {
            UpdateCudaProviderOptions(nativeProviderOptions, providerOptions);
            Ort.Ok(Ort.SessionOptionsAppendExecutionProvider_CUDA_V2(
                Pointer,
                nativeProviderOptions));
        }
        finally
        {
            Ort.ReleaseCUDAProviderOptions(nativeProviderOptions);
        }
    }

    public unsafe void AppendTensorRtExecutionProvider(IReadOnlyDictionary<string, string>? providerOptions = null)
    {
        ThrowIfDisposed();
        Ort.OrtTensorRTProviderOptionsV2* nativeProviderOptions;
        Ort.Ok(Ort.CreateTensorRTProviderOptions(&nativeProviderOptions));
        try
        {
            UpdateTensorRtProviderOptions(nativeProviderOptions, providerOptions);
            Ort.Ok(Ort.SessionOptionsAppendExecutionProvider_TensorRT_V2(
                Pointer,
                nativeProviderOptions));
        }
        finally
        {
            Ort.ReleaseTensorRTProviderOptions(nativeProviderOptions);
        }
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    internal unsafe Ort.OrtSessionOptions* Pointer => (Ort.OrtSessionOptions*)handle;

    protected override unsafe bool ReleaseHandle()
    {
        Ort.ReleaseSessionOptions((Ort.OrtSessionOptions*)handle);
        return true;
    }

    unsafe void InvokeUtf8(
        string value,
        Utf8Action action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        var utf8Value = Utf8StringMarshaller.ConvertToUnmanaged(value);
        try
        {
            Ort.Ok(action(Pointer, (sbyte*)utf8Value));
        }
        finally
        {
            Utf8StringMarshaller.Free(utf8Value);
        }
    }

    static unsafe void UpdateCudaProviderOptions(
        Ort.OrtCUDAProviderOptionsV2* nativeOptions,
        IReadOnlyDictionary<string, string>? options)
    {
        if (options is null || options.Count == 0)
        {
            return;
        }

        var keys = stackalloc sbyte*[options.Count];
        var values = stackalloc sbyte*[options.Count];
        var initializedCount = 0;
        try
        {
            foreach (var option in options)
            {
                keys[initializedCount] = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(option.Key);
                values[initializedCount] = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(option.Value);
                ++initializedCount;
            }
            Ort.Ok(Ort.UpdateCUDAProviderOptions(nativeOptions, keys, values, (nuint)options.Count));
        }
        finally
        {
            for (var index = 0; index < initializedCount; ++index)
            {
                Utf8StringMarshaller.Free((byte*)values[index]);
                Utf8StringMarshaller.Free((byte*)keys[index]);
            }
        }
    }

    static unsafe void UpdateTensorRtProviderOptions(
        Ort.OrtTensorRTProviderOptionsV2* nativeOptions,
        IReadOnlyDictionary<string, string>? options)
    {
        if (options is null || options.Count == 0)
        {
            return;
        }

        var keys = stackalloc sbyte*[options.Count];
        var values = stackalloc sbyte*[options.Count];
        var initializedCount = 0;
        try
        {
            foreach (var option in options)
            {
                keys[initializedCount] = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(option.Key);
                values[initializedCount] = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(option.Value);
                ++initializedCount;
            }
            Ort.Ok(Ort.UpdateTensorRTProviderOptions(nativeOptions, keys, values, (nuint)options.Count));
        }
        finally
        {
            for (var index = 0; index < initializedCount; ++index)
            {
                Utf8StringMarshaller.Free((byte*)values[index]);
                Utf8StringMarshaller.Free((byte*)keys[index]);
            }
        }
    }

    void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(IsClosed || IsInvalid, this);

    unsafe delegate Ort.OrtStatusHandle Utf8Action(Ort.OrtSessionOptions* options, sbyte* value);
}
