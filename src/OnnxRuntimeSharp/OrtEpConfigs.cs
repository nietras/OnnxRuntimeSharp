using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OnnxRuntimeSharp;

/// <summary>Execution-provider configurations and opt-in runtime availability checks.</summary>
public static unsafe class OrtEpConfigs
{
    internal const string TensorRTName = nameof(TensorRT);
    internal const string CUDAName = nameof(CUDA);
    internal const string DirectMLName = nameof(DirectML);
    internal const string OpenVINOName = nameof(OpenVINO);

    internal const string NVidiaTF32OverrideName = "NVIDIA_TF32_OVERRIDE";
    internal const string NVidiaTF32OverrideDisable = "0";
    internal const string CUDA_MODULE_LOADING = nameof(CUDA_MODULE_LOADING);
    internal const string LAZY = nameof(LAZY);

    // ONNX IR 8 / opset 13: sum = Add(first, second), each a float tensor of shape [1].
    // Both inputs are runtime values, so the computation cannot be constant-folded.
    internal static ReadOnlySpan<byte> ProbeModelOnnxBytes =>
    [
        0x08, 0x08, 0x12, 0x10, 0x4F, 0x6E, 0x6E, 0x78, 0x52, 0x75, 0x6E, 0x74, 0x69, 0x6D, 0x65, 0x53,
        0x68, 0x61, 0x72, 0x70, 0x3A, 0x5E, 0x0A, 0x19, 0x0A, 0x05, 0x66, 0x69, 0x72, 0x73, 0x74, 0x0A,
        0x06, 0x73, 0x65, 0x63, 0x6F, 0x6E, 0x64, 0x12, 0x03, 0x73, 0x75, 0x6D, 0x22, 0x03, 0x41, 0x64,
        0x64, 0x12, 0x03, 0x41, 0x64, 0x64, 0x5A, 0x13, 0x0A, 0x05, 0x66, 0x69, 0x72, 0x73, 0x74, 0x12,
        0x0A, 0x0A, 0x08, 0x08, 0x01, 0x12, 0x04, 0x0A, 0x02, 0x08, 0x01, 0x5A, 0x14, 0x0A, 0x06, 0x73,
        0x65, 0x63, 0x6F, 0x6E, 0x64, 0x12, 0x0A, 0x0A, 0x08, 0x08, 0x01, 0x12, 0x04, 0x0A, 0x02, 0x08,
        0x01, 0x62, 0x11, 0x0A, 0x03, 0x73, 0x75, 0x6D, 0x12, 0x0A, 0x0A, 0x08, 0x08, 0x01, 0x12, 0x04,
        0x0A, 0x02, 0x08, 0x01, 0x42, 0x02, 0x10, 0x0D,
    ];

    public static OrtEpConfig TensorRT { get; } = new(TensorRTName, options =>
        {
            RequireFunctions(Ort.Api->CreateTensorRTProviderOptions != null &&
                Ort.Api->UpdateTensorRTProviderOptions != null &&
                Ort.Api->SessionOptionsAppendExecutionProvider_TensorRT_V2 != null &&
                Ort.Api->ReleaseTensorRTProviderOptions != null, TensorRTName);
            DisableNVidiaTF32IfNotSet();
            EnableNVidiaCudaModuleLoadingLazyIfNotSet();
            options.AppendExecutionProvider_TensorRT();
        });

    public static OrtEpConfig CUDA { get; } = new(CUDAName, options =>
        {
            RequireFunctions(Ort.Api->CreateCUDAProviderOptions != null &&
                Ort.Api->UpdateCUDAProviderOptions != null &&
                Ort.Api->SessionOptionsAppendExecutionProvider_CUDA_V2 != null &&
                Ort.Api->ReleaseCUDAProviderOptions != null, CUDAName);
            DisableNVidiaTF32IfNotSet();
            EnableNVidiaCudaModuleLoadingLazyIfNotSet();
            options.AppendExecutionProvider_CUDA();
        });

    public static OrtEpConfig DirectML { get; } = new(DirectMLName, (environment, options) =>
        {
            RequireDeviceFunctions(DirectMLName);
            options.SetMemoryPatternEnabled(false);
            options.SetExecutionMode(Ort.ExecutionMode.ORT_SEQUENTIAL);
            var device = environment.GetExecutionProviderDevices()
                .FirstOrDefault(item => item.ExecutionProviderName == "DmlExecutionProvider");
            if (device is null)
            {
                throw new NotSupportedException(
                    "DirectML has no supported device in this environment.");
            }
            options.AppendExecutionProvider(environment, [device]);
        });

    public static OrtEpConfig OpenVINO { get; } = CreateOpenVINO();

    public static OrtEpConfig CPU { get; } = new("CPU", options =>
        options.SetCpuMemoryArenaEnabled(true), allowsCpuFallback: true);

    public static OrtEpConfig CPUSingleThread { get; } = new("CPU(1*InterThread-1*IntraThread)", options =>
        {
            options.SetInterOpThreadCount(1);
            options.SetIntraOpThreadCount(1);
        }, allowsCpuFallback: true);

    /// <summary>No explicit provider configuration; ONNX Runtime uses its default CPU provider.</summary>
    public static OrtEpConfig None { get; } = new("None", _ => { }, allowsCpuFallback: true);

    public static IReadOnlyList<OrtEpConfig> DefaultPrioritizedList { get; } =
        Array.AsReadOnly([TensorRT, CUDA, DirectML, OpenVINO, CPU, None]);

    /// <summary>Sets the process NVIDIA_TF32_OVERRIDE to 0 only when unset.</summary>
    public static void DisableNVidiaTF32IfNotSet()
    {
        // Disable TF32 mode by setting an environment variable.
        // By default CUDA will use TF32 when available.
        // However, TF32 gives different results due to lower precision,
        // and provides no speedup on RTX 30 series for example.
        // https://docs.nvidia.com/deeplearning/tensorrt/release-notes/tensorrt-7.html
        // Only the very expensive A100 GPU provides a significant speedup using TF32
        // https://blogs.nvidia.com/blog/2020/05/14/tensorfloat-32-precision-format/
        var overrideText = Environment.GetEnvironmentVariable(NVidiaTF32OverrideName);
        if (overrideText == null)
        {
            Environment.SetEnvironmentVariable(NVidiaTF32OverrideName, NVidiaTF32OverrideDisable);
            Trace.WriteLine($"{NVidiaTF32OverrideName} not set, " +
                $"setting it to '{NVidiaTF32OverrideDisable}' to disable use of TF32");
        }
        else
        {
            Trace.WriteLine($"{NVidiaTF32OverrideName} already set to '{overrideText}'");
        }
    }

    /// <summary>Sets the process CUDA_MODULE_LOADING to LAZY only when unset.</summary>
    public static void EnableNVidiaCudaModuleLoadingLazyIfNotSet()
    {
        // https://docs.nvidia.com/cuda/cuda-c-programming-guide/index.html#lazy-loading
        // Lazy Loading delays loading of CUDA modules and kernels from program
        // initialization closer to kernels execution. If a program does not use every
        // single kernel it has included, then some kernels will be loaded
        // unnecessarily. This is very common, especially if you include any libraries.
        // Most of the time, programs only use a small amount of kernels from libraries
        // they include.
        //
        // Thanks to Lazy Loading, programs are able to only load kernels they are
        // actually going to use, saving time on initialization. This reduces memory
        // overhead, both on GPU memory and host memory
        //
        // Lazy Loading is enabled by setting the CUDA_MODULE_LOADING environment
        // variable to LAZY.
        //
        // Firstly, CUDA Runtime will no longer load all modules during program
        // initialization, with the exception of modules containing managed variables.
        // Each module will be loaded on first usage of a variable or a kernel from that
        // module. This optimization is only relevant to CUDA Runtime users, CUDA Driver
        // users are unaffected. This optimization shipped in CUDA 11.8.
        //
        // Secondly, loading a module (cuModuleLoad*() family of functions) will not be
        // loading kernels immediately, instead it will delay loading of a kernel until
        // cuModuleGetFunction() is called. There are certain exceptions here, some
        // kernels have to be loaded during cuModuleLoad*(), such as kernels of which
        // pointers are stored in global variables. This optimization is relevant to
        // both CUDA Runtime and CUDA Driver users. CUDA Runtime will only call
        // cuModuleGetFunction() when a kernel is used/referenced for the first time.
        // This optimization shipped in CUDA 11.7.
        //
        // Both of these optimizations are designed to be invisible to the user,
        // assuming CUDA Programming Model is followed.
        var value = Environment.GetEnvironmentVariable(CUDA_MODULE_LOADING);
        if (value == null)
        {
            Environment.SetEnvironmentVariable(CUDA_MODULE_LOADING, LAZY);
            Trace.WriteLine($"{CUDA_MODULE_LOADING} not set, " +
                            $"setting it to '{CUDA_MODULE_LOADING}' to enable lazy loading kernels.");
        }
        else
        {
            Trace.WriteLine($"{CUDA_MODULE_LOADING} already set to '{value}'");
        }
    }

    /// <summary>Creates an OpenVINO configuration using the dedicated V2 append API.</summary>
    public static OrtEpConfig CreateOpenVINO(IReadOnlyDictionary<string, string>? providerOptions = null)
    {
        return new OrtEpConfig(OpenVINOName, options =>
        {
            RequireFunctions(Ort.Api->SessionOptionsAppendExecutionProvider_OpenVINO_V2 != null, OpenVINOName);
            options.AppendExecutionProvider_OpenVINO(providerOptions);
        });
    }

    public static IReadOnlyList<OrtEpConfig> FindAvailablePrioritizedExecutionProviders(
        IReadOnlyList<OrtEpConfig>? prioritizedExecutionProvidersToTry = null)
    {
        var environment = OrtEnv.Instance();
        return FindAvailablePrioritizedExecutionProviders(environment, prioritizedExecutionProvidersToTry);
    }

    /// <summary>Executes the smoke test eagerly and returns successful configurations in input order.</summary>
    public static IReadOnlyList<OrtEpConfig> FindAvailablePrioritizedExecutionProviders(
        OrtEnv environment, IReadOnlyList<OrtEpConfig>? prioritizedExecutionProvidersToTry = null)
        => ProbeExecutionProviders(environment, prioritizedExecutionProvidersToTry)
            .Where(result => result.IsAvailable).Select(result => result.Provider).ToArray();

    public static IReadOnlyList<OrtEpProbeResult> ProbeExecutionProviders(
        IReadOnlyList<OrtEpConfig>? prioritizedExecutionProvidersToTry = null)
    {
        var environment = OrtEnv.Instance();
        return ProbeExecutionProviders(environment, prioritizedExecutionProvidersToTry);
    }

    /// <summary>Tests each configuration with real inference and preserves failures for diagnostics.</summary>
    /// <remarks>
    /// Success applies only to this small float Add model, device and configuration, not every model.
    /// Non-CPU configurations are tested without CPU fallback. Native crashes cannot be caught.
    /// Custom append delegates must only call APIs supported by the loaded runtime.
    /// </remarks>
    public static IReadOnlyList<OrtEpProbeResult> ProbeExecutionProviders(
        OrtEnv environment, IReadOnlyList<OrtEpConfig>? prioritizedExecutionProvidersToTry = null)
    {
        ArgumentNullException.ThrowIfNull(environment);
        ObjectDisposedException.ThrowIf(environment.IsClosed || environment.IsInvalid, environment);
        // Older runtimes accept unknown config keys but do not enforce CPU-fallback disabling.
        RequireApiVersion(16);
        var providers = prioritizedExecutionProvidersToTry ?? DefaultPrioritizedList;
        var results = new List<OrtEpProbeResult>(providers.Count);
        foreach (var provider in providers)
        {
            try
            {
                using var options = new OrtSessionOptions();
                options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_DISABLE_ALL);
                options.SetIntraOpThreadCount(1);
                options.SetInterOpThreadCount(1);
                if (!provider.AllowsCpuFallback)
                {
                    options.AddConfigEntry("session.disable_cpu_ep_fallback", "1");
                }
                provider.Append(environment, options);
                using var session = new OrtSession(environment, ProbeModelOnnxBytes, options);
                RunProbe(session);
                results.Add(new(provider, null));
            }
            catch (Exception exception) when (exception is not OutOfMemoryException)
            {
                results.Add(new(provider, exception));
            }
        }
        return results.AsReadOnly();
    }

    static void RunProbe(OrtSession session)
    {
        using var first = new OrtValue<float>([2], [1]);
        using var second = new OrtValue<float>([3], [1]);
        using var output = new OrtValue<float>([0], [1]);
        var inputs = new[] { session.CreateInputBinding(0, first), session.CreateInputBinding(1, second) };
        var outputs = new[] { session.CreateOutputBinding(0, output) };
        session.Run(inputs, outputs);
        if (output.Data[0] != 5f)
        {
            throw new InvalidOperationException(
                "Execution-provider probe returned an incorrect sum.");
        }
        first.Data[0] = -4;
        session.Run(inputs, outputs);
        if (output.Data[0] != -1f)
        {
            throw new InvalidOperationException(
                "Execution-provider probe returned an incorrect sum after changing the input.");
        }
    }

    static void RequireApiVersion(uint minimum)
    {
        if (Ort.ApiVersion < minimum)
        {
            throw new NotSupportedException(
                "This execution-provider configuration requires " +
                $"ONNX Runtime API {minimum} or later; " +
                $"loaded API is {Ort.ApiVersion}.");
        }
    }

    static void RequireDeviceFunctions(string providerName)
    {
        RequireFunctions(Ort.Api->GetEpDevices != null &&
            Ort.Api->SessionOptionsAppendExecutionProvider_V2 != null &&
            Ort.Api->EpDevice_EpName != null && Ort.Api->EpDevice_EpVendor != null &&
            Ort.Api->EpDevice_EpMetadata != null && Ort.Api->EpDevice_EpOptions != null &&
            Ort.Api->EpDevice_Device != null && Ort.Api->HardwareDevice_Type != null &&
            Ort.Api->HardwareDevice_VendorId != null && Ort.Api->HardwareDevice_Vendor != null &&
            Ort.Api->HardwareDevice_DeviceId != null && Ort.Api->HardwareDevice_Metadata != null &&
            Ort.Api->GetKeyValuePairs != null, providerName);
    }

    static void RequireFunctions(bool supported, string providerName)
    {
        if (!supported)
        {
            throw new NotSupportedException(
                $"The loaded ONNX Runtime API {Ort.ApiVersion} does not expose " +
                $"the functions required by {providerName}.");
        }
    }
}
