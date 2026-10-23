using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OnnxRuntimeSharp;

/// <summary>Execution-provider configurations and their default priority.</summary>
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

    public static OrtEpConfig DirectML { get; } = new(DirectMLName, (env, options) =>
        {
            RequireDeviceFunctions(DirectMLName);
            options.SetMemoryPatternEnabled(false);
            options.SetExecutionMode(Ort.ExecutionMode.ORT_SEQUENTIAL);
            var device = env.GetExecutionProviderDevices()
                .FirstOrDefault(item => item.ExecutionProviderName == "DmlExecutionProvider");
            if (device is null)
            {
                throw new NotSupportedException(
                    "DirectML has no supported device in this env.");
            }
            options.AppendExecutionProvider(env, [device]);
        });

    public static OrtEpConfig OpenVINO { get; } = CreateOpenVINO(OpenVINOName);

    public static OrtEpConfig OpenVINOCPUSingleThread { get; } = CreateOpenVINOCPU(
        $"{OpenVINOName} CPU 1×Thread 1×Stream", threadCount: 1, streamCount: 1);

    public static OrtEpConfig CPU { get; } = new("CPU", options =>
        options.SetCpuMemoryArenaEnabled(true), allowsCpuFallback: true);

    public static OrtEpConfig CPUSingleThread { get; } = new("CPU 1×InterThread 1×IntraThread", options =>
        {
            options.SetInterOpThreadCount(1);
            options.SetIntraOpThreadCount(1);
        }, allowsCpuFallback: true);

    /// <summary>No explicit provider configuration; ONNX Runtime uses its default CPU provider.</summary>
    public static OrtEpConfig None { get; } = new("None", _ => { }, allowsCpuFallback: true);

    public static IReadOnlyList<OrtEpConfig> DefaultPrioritizedList { get; } =
        Array.AsReadOnly([TensorRT, CUDA, DirectML, OpenVINO, CPU, None]);

    public static OrtEpConfig CreateOpenVINOCPU(int threadCount, int streamCount) =>
        CreateOpenVINOCPU($"{OpenVINOName} CPU {threadCount}×Threads {streamCount}×Streams",
                          threadCount, streamCount);

    public static OrtEpConfig CreateOpenVINOCPU(string name, int threadCount, int streamCount) =>
        CreateOpenVINO(name, new Dictionary<string, string>
        {
            { "device_type", "CPU" },
            { "num_of_threads", threadCount.ToString() },
            { "num_streams", streamCount.ToString() },
        });

    public static OrtEpConfig CreateOpenVINO(string name,
        IReadOnlyDictionary<string, string>? providerOptions = null)
    {
        return new OrtEpConfig(name, options =>
        {
            RequireFunctions(Ort.Api->SessionOptionsAppendExecutionProvider_OpenVINO_V2 != null, name);
            // Do not use ORT built-in optimizations, let OpenVINO handle all that.
            options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_DISABLE_ALL);
            options.AppendExecutionProvider_OpenVINO(providerOptions);
        });
    }

    /// <summary>Sets the process NVIDIA_TF32_OVERRIDE to 0 only when unset.</summary>
    static void DisableNVidiaTF32IfNotSet()
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
    static void EnableNVidiaCudaModuleLoadingLazyIfNotSet()
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

    static void RequireFunctions(bool supported, string providerName,
        [CallerArgumentExpression(nameof(supported))] string supportedExpression = "")
    {
        if (!supported)
        {
            throw new NotSupportedException(
                $"The loaded ONNX Runtime API {Ort.ApiVersion} does not expose " +
                $"the functions required by '{providerName}': '{supportedExpression}'.");
        }
    }
}
