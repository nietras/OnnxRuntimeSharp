using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

public sealed class OrtSessionOptions : OrtSafeHandle<Ort.OrtSessionOptionsHandle>
{
    public OrtSessionOptions()
    {
        unsafe
        {
            Ort.OrtSessionOptionsHandle options;
            Ort.CreateSessionOptions(&options).Ok();
            SetHandle(options.Value);
            Ort.SetSessionGraphOptimizationLevel(options, Ort.GraphOptimizationLevel.ORT_ENABLE_ALL).Ok();
        }
    }

    public unsafe void EnableProfiling(string profileFilePrefix)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(profileFilePrefix);
        fixed (char* pathPointer = profileFilePrefix)
        {
            using var nativePath = new OrtNativePath(profileFilePrefix, pathPointer);
            Ort.Ok(Ort.EnableProfiling(Handle, nativePath.Pointer));
        }
    }

    public unsafe void DisableProfiling()
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.DisableProfiling(Handle));
    }

    public unsafe void SetIntraOpThreadCount(int threadCount)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(threadCount);
        Ort.Ok(Ort.SetIntraOpNumThreads(Handle, threadCount));
    }

    public unsafe void SetInterOpThreadCount(int threadCount)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(threadCount);
        Ort.Ok(Ort.SetInterOpNumThreads(Handle, threadCount));
    }

    public unsafe void SetGraphOptimizationLevel(Ort.GraphOptimizationLevel graphOptimizationLevel)
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SetSessionGraphOptimizationLevel(Handle, graphOptimizationLevel));
    }

    public unsafe void SetExecutionMode(Ort.ExecutionMode executionMode)
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SetSessionExecutionMode(Handle, executionMode));
    }

    public unsafe void SetMemoryPatternEnabled(bool enabled)
    {
        ThrowIfDisposed();
        Ort.Ok(enabled ? Ort.EnableMemPattern(Handle) : Ort.DisableMemPattern(Handle));
    }

    public unsafe void SetCpuMemoryArenaEnabled(bool enabled)
    {
        ThrowIfDisposed();
        Ort.Ok(enabled ? Ort.EnableCpuMemArena(Handle) : Ort.DisableCpuMemArena(Handle));
    }

    public unsafe void SetDeterministicCompute(bool enabled)
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SetDeterministicCompute(Handle, enabled ? (byte)1 : (byte)0));
    }

    public unsafe void DisablePerSessionThreads()
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.DisablePerSessionThreads(Handle));
    }

    public unsafe void SetLogVerbosityLevel(int level)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegative(level);
        Ort.Ok(Ort.SetSessionLogVerbosityLevel(Handle, level));
    }

    public unsafe void SetLogSeverityLevel(Ort.OrtLoggingLevel level)
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SetSessionLogSeverityLevel(Handle, (int)level));
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
            Ort.Ok(Ort.AddSessionConfigEntry(Handle, (sbyte*)utf8Key, (sbyte*)utf8Value));
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
        fixed (char* pathPointer = path)
        {
            using var nativePath = new OrtNativePath(path, pathPointer);
            Ort.Ok(Ort.SetOptimizedModelFilePath(Handle, nativePath.Pointer));
        }
    }

    public unsafe void AppendExecutionProvider(string providerName)
        => AppendExecutionProvider(providerName, null);

    public unsafe void AppendExecutionProvider_OpenVINO(IReadOnlyDictionary<string, string>? providerOptions = null)
    {
        ThrowIfDisposed();
        var optionCount = providerOptions?.Count ?? 0;
        var keys = stackalloc sbyte*[optionCount];
        var values = stackalloc sbyte*[optionCount];
        using var nativePairs = new OrtUtf8KeyValuePairs(providerOptions, keys, values, optionCount);

        Ort.Ok(Ort.SessionOptionsAppendExecutionProvider_OpenVINO_V2(
            Handle,
            nativePairs.Keys,
            nativePairs.Values,
            nativePairs.Count));
    }

    public unsafe void AppendExecutionProvider(
        string providerName,
        IReadOnlyDictionary<string, string>? providerOptions)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);
        var optionCount = providerOptions?.Count ?? 0;
        var keys = stackalloc sbyte*[optionCount];
        var values = stackalloc sbyte*[optionCount];
        var utf8ProviderName = Utf8StringMarshaller.ConvertToUnmanaged(providerName);
        try
        {
            using var nativePairs = new OrtUtf8KeyValuePairs(providerOptions, keys, values, optionCount);

            Ort.Ok(Ort.SessionOptionsAppendExecutionProvider(
                Handle,
                (sbyte*)utf8ProviderName,
                nativePairs.Keys,
                nativePairs.Values,
                nativePairs.Count));
        }
        finally
        {
            Utf8StringMarshaller.Free(utf8ProviderName);
        }
    }

    public unsafe void AppendExecutionProvider(
        OrtEnv environment,
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
        var environmentReferenceAdded = false;
        try
        {
            environment.DangerousAddRef(ref environmentReferenceAdded);
            using var nativePairs = new OrtUtf8KeyValuePairs(providerOptions, keys, values, optionCount);
            Ort.Ok(Ort.SessionOptionsAppendExecutionProvider_V2(
                Handle,
                environment.Handle,
                nativeDevices,
                (nuint)devices.Length,
                nativePairs.Keys,
                nativePairs.Values,
                nativePairs.Count));
        }
        finally
        {
            if (environmentReferenceAdded)
            {
                environment.DangerousRelease();
            }
        }
    }

    public unsafe void SetExecutionProviderSelectionPolicy(Ort.OrtExecutionProviderDevicePolicy policy)
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SessionOptionsSetEpSelectionPolicy(Handle, policy));
    }

    public unsafe void AppendExecutionProvider_CUDA(IReadOnlyDictionary<string, string>? providerOptions = null)
    {
        ThrowIfDisposed();
        Ort.OrtCUDAProviderOptionsV2* nativeProviderOptions;
        Ort.Ok(Ort.CreateCUDAProviderOptions(&nativeProviderOptions));
        try
        {
            UpdateCudaProviderOptions(nativeProviderOptions, providerOptions);
            Ort.Ok(Ort.SessionOptionsAppendExecutionProvider_CUDA_V2(
                Handle,
                nativeProviderOptions));
        }
        finally
        {
            Ort.ReleaseCUDAProviderOptions(nativeProviderOptions);
        }
    }

    public unsafe void AppendExecutionProvider_TensorRT(IReadOnlyDictionary<string, string>? providerOptions = null)
    {
        ThrowIfDisposed();
        Ort.OrtTensorRTProviderOptionsV2* nativeProviderOptions;
        Ort.Ok(Ort.CreateTensorRTProviderOptions(&nativeProviderOptions));
        try
        {
            UpdateTensorRtProviderOptions(nativeProviderOptions, providerOptions);
            Ort.Ok(Ort.SessionOptionsAppendExecutionProvider_TensorRT_V2(
                Handle,
                nativeProviderOptions));
        }
        finally
        {
            Ort.ReleaseTensorRTProviderOptions(nativeProviderOptions);
        }
    }

    protected override unsafe bool ReleaseHandle()
    {
        Ort.ReleaseSessionOptions(Handle);
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
            Ort.Ok(action(Handle, (sbyte*)utf8Value));
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

        var optionCount = options.Count;
        var keys = stackalloc sbyte*[optionCount];
        var values = stackalloc sbyte*[optionCount];
        using var nativePairs = new OrtUtf8KeyValuePairs(options, keys, values, optionCount);
        Ort.Ok(Ort.UpdateCUDAProviderOptions(nativeOptions, nativePairs.Keys, nativePairs.Values, nativePairs.Count));
    }

    static unsafe void UpdateTensorRtProviderOptions(
        Ort.OrtTensorRTProviderOptionsV2* nativeOptions,
        IReadOnlyDictionary<string, string>? options)
    {
        if (options is null || options.Count == 0)
        {
            return;
        }

        var optionCount = options.Count;
        var keys = stackalloc sbyte*[optionCount];
        var values = stackalloc sbyte*[optionCount];
        using var nativePairs = new OrtUtf8KeyValuePairs(options, keys, values, optionCount);
        Ort.Ok(Ort.UpdateTensorRTProviderOptions(nativeOptions, nativePairs.Keys, nativePairs.Values, nativePairs.Count));
    }

    unsafe delegate Ort.OrtStatusHandle Utf8Action(Ort.OrtSessionOptionsHandle options, sbyte* value);
}
