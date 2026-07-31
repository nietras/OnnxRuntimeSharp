using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

public sealed class OrtSessionOptions : SafeHandle
{
    public OrtSessionOptions()
        : base(nint.Zero, ownsHandle: true)
    {
        unsafe
        {
            Ort.OrtSessionOptions* options;
            Ort.ThrowIfError(Ort.CreateSessionOptions(&options));
            SetHandle((nint)options);
        }
    }

    public unsafe void EnableProfiling(string profileFilePrefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(profileFilePrefix);
        if (OperatingSystem.IsWindows())
        {
            fixed (char* utf16Prefix = profileFilePrefix)
            {
                Ort.ThrowIfError(Ort.EnableProfiling((Ort.OrtSessionOptions*)handle, (ushort*)utf16Prefix));
            }

            return;
        }

        var utf8Prefix = Utf8StringMarshaller.ConvertToUnmanaged(profileFilePrefix);
        try
        {
            Ort.ThrowIfError(Ort.EnableProfiling((Ort.OrtSessionOptions*)handle, (ushort*)utf8Prefix));
        }
        finally
        {
            Utf8StringMarshaller.Free(utf8Prefix);
        }
    }

    public unsafe void SetIntraOpThreadCount(int threadCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(threadCount);
        Ort.ThrowIfError(Ort.SetIntraOpNumThreads((Ort.OrtSessionOptions*)handle, threadCount));
    }

    public unsafe void SetInterOpThreadCount(int threadCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(threadCount);
        Ort.ThrowIfError(Ort.SetInterOpNumThreads((Ort.OrtSessionOptions*)handle, threadCount));
    }

    public unsafe void SetGraphOptimizationLevel(Ort.GraphOptimizationLevel graphOptimizationLevel) =>
        Ort.ThrowIfError(Ort.SetSessionGraphOptimizationLevel((Ort.OrtSessionOptions*)handle, graphOptimizationLevel));

    public unsafe void AppendExecutionProvider(string providerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);
        if (string.Equals(providerName, "CUDAExecutionProvider", StringComparison.Ordinal))
        {
            AppendCudaExecutionProvider();
            return;
        }
        if (string.Equals(providerName, "TensorrtExecutionProvider", StringComparison.Ordinal))
        {
            AppendTensorRtExecutionProvider();
            return;
        }

        var utf8ProviderName = Utf8StringMarshaller.ConvertToUnmanaged(providerName);
        try
        {
            Ort.ThrowIfError(Ort.SessionOptionsAppendExecutionProvider(
                (Ort.OrtSessionOptions*)handle,
                (sbyte*)utf8ProviderName,
                null,
                null,
                0));
        }
        finally
        {
            Utf8StringMarshaller.Free(utf8ProviderName);
        }
    }

    public unsafe void AppendCudaExecutionProvider()
    {
        Ort.OrtCUDAProviderOptionsV2* providerOptions;
        Ort.ThrowIfError(Ort.CreateCUDAProviderOptions(&providerOptions));
        try
        {
            Ort.ThrowIfError(Ort.SessionOptionsAppendExecutionProvider_CUDA_V2(
                (Ort.OrtSessionOptions*)handle,
                providerOptions));
        }
        finally
        {
            Ort.ReleaseCUDAProviderOptions(providerOptions);
        }
    }

    public unsafe void AppendTensorRtExecutionProvider()
    {
        Ort.OrtTensorRTProviderOptionsV2* providerOptions;
        Ort.ThrowIfError(Ort.CreateTensorRTProviderOptions(&providerOptions));
        try
        {
            Ort.ThrowIfError(Ort.SessionOptionsAppendExecutionProvider_TensorRT_V2(
                (Ort.OrtSessionOptions*)handle,
                providerOptions));
        }
        finally
        {
            Ort.ReleaseTensorRTProviderOptions(providerOptions);
        }
    }

    public override bool IsInvalid => handle == nint.Zero;

    protected override unsafe bool ReleaseHandle()
    {
        Ort.ReleaseSessionOptions((Ort.OrtSessionOptions*)handle);
        return true;
    }
}
