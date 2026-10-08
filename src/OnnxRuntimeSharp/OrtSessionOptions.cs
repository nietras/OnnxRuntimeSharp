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
            Ort.EnableProfiling(Handle, nativePath.Pointer).Ok();
        }
    }

    public unsafe void DisableProfiling()
    {
        ThrowIfDisposed();
        Ort.DisableProfiling(Handle).Ok();
    }

    public unsafe void SetIntraOpThreadCount(int threadCount)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(threadCount);
        Ort.SetIntraOpNumThreads(Handle, threadCount).Ok();
    }

    public unsafe void SetInterOpThreadCount(int threadCount)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(threadCount);
        Ort.SetInterOpNumThreads(Handle, threadCount).Ok();
    }

    public unsafe void SetGraphOptimizationLevel(Ort.GraphOptimizationLevel graphOptimizationLevel)
    {
        ThrowIfDisposed();
        Ort.SetSessionGraphOptimizationLevel(Handle, graphOptimizationLevel).Ok();
    }

    public unsafe void SetExecutionMode(Ort.ExecutionMode executionMode)
    {
        ThrowIfDisposed();
        Ort.SetSessionExecutionMode(Handle, executionMode).Ok();
    }

    public unsafe void SetMemoryPatternEnabled(bool enabled)
    {
        ThrowIfDisposed();
        (enabled ? Ort.EnableMemPattern(Handle) : Ort.DisableMemPattern(Handle)).Ok();
    }

    public unsafe void SetCpuMemoryArenaEnabled(bool enabled)
    {
        ThrowIfDisposed();
        (enabled ? Ort.EnableCpuMemArena(Handle) : Ort.DisableCpuMemArena(Handle)).Ok();
    }

    public unsafe void SetDeterministicCompute(bool enabled)
    {
        ThrowIfDisposed();
        Ort.SetDeterministicCompute(Handle, enabled ? (byte)1 : (byte)0).Ok();
    }

    public unsafe void DisablePerSessionThreads()
    {
        ThrowIfDisposed();
        Ort.DisablePerSessionThreads(Handle).Ok();
    }

    public unsafe void SetLogVerbosityLevel(int level)
    {
        ThrowIfDisposed();
        ArgumentOutOfRangeException.ThrowIfNegative(level);
        Ort.SetSessionLogVerbosityLevel(Handle, level).Ok();
    }

    public unsafe void SetLogSeverityLevel(Ort.OrtLoggingLevel level)
    {
        ThrowIfDisposed();
        Ort.SetSessionLogSeverityLevel(Handle, (int)level).Ok();
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
            Ort.AddSessionConfigEntry(Handle, (sbyte*)utf8Key, (sbyte*)utf8Value).Ok();
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
            Ort.SetOptimizedModelFilePath(Handle, nativePath.Pointer).Ok();
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

        Ort.SessionOptionsAppendExecutionProvider_OpenVINO_V2(
            Handle,
            nativePairs.Keys,
            nativePairs.Values,
            nativePairs.Count).Ok();
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

            Ort.SessionOptionsAppendExecutionProvider(
                Handle,
                (sbyte*)utf8ProviderName,
                nativePairs.Keys,
                nativePairs.Values,
                nativePairs.Count).Ok();
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
            Ort.SessionOptionsAppendExecutionProvider_V2(
                Handle,
                environment.Handle,
                nativeDevices,
                (nuint)devices.Length,
                nativePairs.Keys,
                nativePairs.Values,
                nativePairs.Count).Ok();
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
        Ort.SessionOptionsSetEpSelectionPolicy(Handle, policy).Ok();
    }

    public unsafe void AppendExecutionProvider_CUDA(IReadOnlyDictionary<string, string>? providerOptions = null)
    {
        ThrowIfDisposed();
        Ort.OrtCUDAProviderOptionsV2* nativeProviderOptions;
        Ort.CreateCUDAProviderOptions(&nativeProviderOptions).Ok();
        try
        {
            UpdateCudaProviderOptions(nativeProviderOptions, providerOptions);
            Ort.SessionOptionsAppendExecutionProvider_CUDA_V2(
                Handle,
                nativeProviderOptions).Ok();
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
        Ort.CreateTensorRTProviderOptions(&nativeProviderOptions).Ok();
        try
        {
            UpdateTensorRtProviderOptions(nativeProviderOptions, providerOptions);
            Ort.SessionOptionsAppendExecutionProvider_TensorRT_V2(
                Handle,
                nativeProviderOptions).Ok();
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
            action(Handle, (sbyte*)utf8Value).Ok();
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
        Ort.UpdateCUDAProviderOptions(nativeOptions, nativePairs.Keys, nativePairs.Values, nativePairs.Count).Ok();
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
        Ort.UpdateTensorRTProviderOptions(nativeOptions, nativePairs.Keys, nativePairs.Values, nativePairs.Count).Ok();
    }

    unsafe delegate Ort.OrtStatusHandle Utf8Action(Ort.OrtSessionOptionsHandle options, sbyte* value);
}
