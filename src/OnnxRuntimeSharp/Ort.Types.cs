using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

[
    AttributeUsage(
        AttributeTargets.Field |
        AttributeTargets.Method |
        AttributeTargets.Parameter |
        AttributeTargets.ReturnValue,
        Inherited = false)
]
sealed class NativeTypeNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

public static unsafe partial class Ort
{
    public readonly struct OrtEpApi;
    public readonly struct OrtExternalMemoryHandle;
    public readonly struct OrtExternalSemaphoreHandle;

    public enum OrtErrorCode
    {
        ORT_OK,
        ORT_FAIL,
        ORT_INVALID_ARGUMENT,
        ORT_NO_SUCHFILE,
        ORT_NO_MODEL,
        ORT_ENGINE_ERROR,
        ORT_RUNTIME_EXCEPTION,
        ORT_INVALID_PROTOBUF,
        ORT_MODEL_LOADED,
        ORT_NOT_IMPLEMENTED,
        ORT_INVALID_GRAPH,
        ORT_EP_FAIL,
        ORT_MODEL_LOAD_CANCELED,
        ORT_MODEL_REQUIRES_COMPILATION,
        ORT_NOT_FOUND,
        ORT_DEVICE_RESET,
    }

    public enum ONNXTensorElementDataType
    {
        ONNX_TENSOR_ELEMENT_DATA_TYPE_UNDEFINED,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT8,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_INT8,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT16,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_INT16,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_INT32,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_INT64,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_STRING,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_BOOL,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT16,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_DOUBLE,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT32,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT64,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_COMPLEX64,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_COMPLEX128,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_BFLOAT16,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT8E4M3FN,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT8E4M3FNUZ,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT8E5M2,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT8E5M2FNUZ,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT4,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_INT4,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT4E2M1,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT2,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_INT2,
        ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT8E8M0,
    }

    public enum ONNXType
    {
        ONNX_TYPE_UNKNOWN,
        ONNX_TYPE_TENSOR,
        ONNX_TYPE_SEQUENCE,
        ONNX_TYPE_MAP,
        ONNX_TYPE_OPAQUE,
        ONNX_TYPE_SPARSETENSOR,
        ONNX_TYPE_OPTIONAL,
    }

    public enum OrtSparseFormat
    {
        ORT_SPARSE_UNDEFINED = 0,
        ORT_SPARSE_COO = 0x1,
        ORT_SPARSE_CSRC = 0x2,
        ORT_SPARSE_BLOCK_SPARSE = 0x4,
    }

    public enum OrtSparseIndicesFormat
    {
        ORT_SPARSE_COO_INDICES,
        ORT_SPARSE_CSR_INNER_INDICES,
        ORT_SPARSE_CSR_OUTER_INDICES,
        ORT_SPARSE_BLOCK_SPARSE_INDICES,
    }

    public enum OrtLoggingLevel
    {
        ORT_LOGGING_LEVEL_VERBOSE,
        ORT_LOGGING_LEVEL_INFO,
        ORT_LOGGING_LEVEL_WARNING,
        ORT_LOGGING_LEVEL_ERROR,
        ORT_LOGGING_LEVEL_FATAL,
    }

    public enum OrtOpAttrType
    {
        ORT_OP_ATTR_UNDEFINED = 0,
        ORT_OP_ATTR_INT,
        ORT_OP_ATTR_INTS,
        ORT_OP_ATTR_FLOAT,
        ORT_OP_ATTR_FLOATS,
        ORT_OP_ATTR_STRING,
        ORT_OP_ATTR_STRINGS,
        ORT_OP_ATTR_GRAPH,
        ORT_OP_ATTR_TENSOR,
    }

    public readonly struct OrtEnv;
    public readonly struct OrtStatusHandle
    {
        internal readonly IntPtr Value;

        internal bool IsNull => Value == IntPtr.Zero;
    }
    public readonly struct OrtMemoryInfo;
    public readonly struct OrtIoBinding;
    public readonly struct OrtSession;
    public readonly struct OrtValue;
    public readonly struct OrtRunOptions;
    public readonly struct OrtTypeInfo;
    public readonly struct OrtTensorTypeAndShapeInfo;
    public readonly struct OrtMapTypeInfo;
    public readonly struct OrtSequenceTypeInfo;
    public readonly struct OrtOptionalTypeInfo;
    public readonly struct OrtSessionOptions;
    public readonly struct OrtCustomOpDomain;
    public readonly struct OrtModelMetadata;
    public readonly struct OrtThreadPoolParams;
    public readonly struct OrtThreadingOptions;
    public readonly struct OrtArenaCfg;
    public readonly struct OrtPrepackedWeightsContainer;
    public readonly struct OrtTensorRTProviderOptionsV2;
    public readonly struct OrtNvTensorRtRtxProviderOptions;
    public readonly struct OrtCUDAProviderOptionsV2;
    public readonly struct OrtCANNProviderOptions;
    public readonly struct OrtDnnlProviderOptions;
    public readonly struct OrtOp;
    public readonly struct OrtOpAttr;
    public readonly struct OrtLogger;
    public readonly struct OrtShapeInferContext;
    public readonly struct OrtLoraAdapter;
    public readonly struct OrtValueInfo;
    public readonly struct OrtNode;
    public readonly struct OrtGraph;
    public readonly struct OrtModel;
    public readonly struct OrtModelCompilationOptions;
    public readonly struct OrtHardwareDevice;
    public readonly struct OrtEpDevice;
    public readonly struct OrtKeyValuePairs;
    public readonly struct OrtSyncStream;
    public readonly struct OrtExternalInitializerInfo;
    public readonly struct OrtExternalResourceImporter;
    public readonly struct OrtDeviceEpIncompatibilityDetails;
    public readonly struct OrtEpAssignedSubgraph;
    public readonly struct OrtEpAssignedNode;

    public unsafe partial struct OrtAllocator
    {
        [NativeTypeName("uint32_t")]
        public uint version;

        [NativeTypeName("void *(*)(struct OrtAllocator *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, nuint, void*> Alloc;

        [NativeTypeName("void (*)(struct OrtAllocator *, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, void*, void> Free;

        [NativeTypeName("const struct OrtMemoryInfo *(*)(const struct OrtAllocator *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, OrtMemoryInfo*> Info;

        [NativeTypeName("void *(*)(struct OrtAllocator *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, nuint, void*> Reserve;

        [NativeTypeName("OrtStatusPtr (*)(const struct OrtAllocator *, OrtKeyValuePairs **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, OrtKeyValuePairs**, OrtStatusHandle> GetStats;

        [NativeTypeName("void *(*)(struct OrtAllocator *, size_t, OrtSyncStream *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, nuint, OrtSyncStream*, void*> AllocOnStream;

        [NativeTypeName("OrtStatusPtr (*)(struct OrtAllocator *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, OrtStatusHandle> Shrink;
    }

    public enum GraphOptimizationLevel
    {
        ORT_DISABLE_ALL = 0,
        ORT_ENABLE_BASIC = 1,
        ORT_ENABLE_EXTENDED = 2,
        ORT_ENABLE_LAYOUT = 3,
        ORT_ENABLE_ALL = 99,
    }

    public enum ExecutionMode
    {
        ORT_SEQUENTIAL = 0,
        ORT_PARALLEL = 1,
    }

    public enum OrtLanguageProjection
    {
        ORT_PROJECTION_C = 0,
        ORT_PROJECTION_CPLUSPLUS = 1,
        ORT_PROJECTION_CSHARP = 2,
        ORT_PROJECTION_PYTHON = 3,
        ORT_PROJECTION_JAVA = 4,
        ORT_PROJECTION_WINML = 5,
        ORT_PROJECTION_NODEJS = 6,
    }

    public readonly struct OrtKernelInfo;
    public readonly struct OrtKernelContext;

    public enum OrtAllocatorType
    {
        OrtInvalidAllocator = -1,
        OrtDeviceAllocator = 0,
        OrtArenaAllocator = 1,
        OrtReadOnlyAllocator = 2,
    }

    public enum OrtMemType
    {
        OrtMemTypeCPUInput = -2,
        OrtMemTypeCPUOutput = -1,
        OrtMemTypeCPU = OrtMemTypeCPUOutput,
        OrtMemTypeDefault = 0,
    }

    public enum OrtDeviceMemoryType
    {
        OrtDeviceMemoryType_DEFAULT = 0,
        OrtDeviceMemoryType_HOST_ACCESSIBLE = 5,
    }

    public enum OrtMemoryInfoDeviceType
    {
        OrtMemoryInfoDeviceType_CPU = 0,
        OrtMemoryInfoDeviceType_GPU = 1,
        OrtMemoryInfoDeviceType_FPGA = 2,
        OrtMemoryInfoDeviceType_NPU = 3,
    }

    public enum OrtHardwareDeviceType
    {
        OrtHardwareDeviceType_CPU,
        OrtHardwareDeviceType_GPU,
        OrtHardwareDeviceType_NPU,
    }

    public enum OrtExecutionProviderDevicePolicy
    {
        OrtExecutionProviderDevicePolicy_DEFAULT,
        OrtExecutionProviderDevicePolicy_PREFER_CPU,
        OrtExecutionProviderDevicePolicy_PREFER_NPU,
        OrtExecutionProviderDevicePolicy_PREFER_GPU,
        OrtExecutionProviderDevicePolicy_MAX_PERFORMANCE,
        OrtExecutionProviderDevicePolicy_MAX_EFFICIENCY,
        OrtExecutionProviderDevicePolicy_MIN_OVERALL_POWER,
    }

    public enum OrtDeviceEpIncompatibilityReason
    {
        OrtDeviceEpIncompatibility_NONE = 0,
        OrtDeviceEpIncompatibility_DRIVER_INCOMPATIBLE = 1 << 0,
        OrtDeviceEpIncompatibility_DEVICE_INCOMPATIBLE = 1 << 1,
        OrtDeviceEpIncompatibility_MISSING_DEPENDENCY = 1 << 2,
        OrtDeviceEpIncompatibility_UNKNOWN = 1 << 31,
    }

    public enum OrtCudnnConvAlgoSearch
    {
        OrtCudnnConvAlgoSearchExhaustive,
        OrtCudnnConvAlgoSearchHeuristic,
        OrtCudnnConvAlgoSearchDefault,
    }

    public unsafe partial struct OrtCUDAProviderOptions
    {
        public int device_id;

        public OrtCudnnConvAlgoSearch cudnn_conv_algo_search;

        [NativeTypeName("size_t")]
        public nuint gpu_mem_limit;

        public int arena_extend_strategy;

        public int do_copy_in_default_stream;

        public int has_user_compute_stream;

        public void* user_compute_stream;

        public OrtArenaCfg* default_memory_arena_cfg;

        public int tunable_op_enable;

        public int tunable_op_tuning_enable;

        public int tunable_op_max_tuning_duration_ms;
    }

    public unsafe partial struct OrtROCMProviderOptions
    {
        public int device_id;

        public int miopen_conv_exhaustive_search;

        [NativeTypeName("size_t")]
        public nuint gpu_mem_limit;

        public int arena_extend_strategy;

        public int do_copy_in_default_stream;

        public int has_user_compute_stream;

        public void* user_compute_stream;

        public OrtArenaCfg* default_memory_arena_cfg;

        public int enable_hip_graph;

        public int tunable_op_enable;

        public int tunable_op_tuning_enable;

        public int tunable_op_max_tuning_duration_ms;
    }

    public unsafe partial struct OrtTensorRTProviderOptions
    {
        public int device_id;

        public int has_user_compute_stream;

        public void* user_compute_stream;

        public int trt_max_partition_iterations;

        public int trt_min_subgraph_size;

        [NativeTypeName("size_t")]
        public nuint trt_max_workspace_size;

        public int trt_fp16_enable;

        public int trt_int8_enable;

        [NativeTypeName("const char *")]
        public sbyte* trt_int8_calibration_table_name;

        public int trt_int8_use_native_calibration_table;

        public int trt_dla_enable;

        public int trt_dla_core;

        public int trt_dump_subgraphs;

        public int trt_engine_cache_enable;

        [NativeTypeName("const char *")]
        public sbyte* trt_engine_cache_path;

        public int trt_engine_decryption_enable;

        [NativeTypeName("const char *")]
        public sbyte* trt_engine_decryption_lib_path;

        public int trt_force_sequential_engine_build;
    }

    public unsafe partial struct OrtMIGraphXProviderOptions
    {
        public int device_id;

        public int migraphx_fp16_enable;

        public int migraphx_fp8_enable;

        public int migraphx_int8_enable;

        public int migraphx_use_native_calibration_table;

        [NativeTypeName("const char *")]
        public sbyte* migraphx_int8_calibration_table_name;

        public int migraphx_save_compiled_model;

        [NativeTypeName("const char *")]
        public sbyte* migraphx_save_model_path;

        public int migraphx_load_compiled_model;

        [NativeTypeName("const char *")]
        public sbyte* migraphx_load_model_path;

        [NativeTypeName("_Bool")]
        public byte migraphx_exhaustive_tune;

        [NativeTypeName("size_t")]
        public nuint migraphx_mem_limit;

        public int migraphx_arena_extend_strategy;
    }

    public unsafe partial struct OrtOpenVINOProviderOptions
    {
        [NativeTypeName("const char *")]
        public sbyte* device_type;

        [NativeTypeName("unsigned char")]
        public byte enable_npu_fast_compile;

        [NativeTypeName("const char *")]
        public sbyte* device_id;

        [NativeTypeName("size_t")]
        public nuint num_of_threads;

        [NativeTypeName("const char *")]
        public sbyte* cache_dir;

        public void* context;

        [NativeTypeName("unsigned char")]
        public byte enable_opencl_throttling;

        [NativeTypeName("unsigned char")]
        public byte enable_dynamic_shapes;
    }

    public readonly struct OrtTrainingApi;

    public unsafe partial struct OrtApiBase
    {
        [NativeTypeName("const OrtApi *(*)(uint32_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<uint, OrtApi*> GetApi;

        [NativeTypeName("const char *(*)(void) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*> GetVersionString;
    }

    public partial struct OrtCustomHandleType
    {
        [NativeTypeName("char")]
        public sbyte __place_holder;
    }

    public unsafe partial struct OrtThreadPoolCallbacksConfig
    {
        [NativeTypeName("uint32_t")]
        public uint version;

        [NativeTypeName("OrtThreadPoolWorkEnqueueFn")]
        public delegate* unmanaged[Cdecl]<void*, void*> on_enqueue;

        [NativeTypeName("OrtThreadPoolWorkStartFn")]
        public delegate* unmanaged[Cdecl]<void*, void*, void> on_start_work;

        [NativeTypeName("OrtThreadPoolWorkStopFn")]
        public delegate* unmanaged[Cdecl]<void*, void*, void> on_stop_work;

        [NativeTypeName("OrtThreadPoolWorkAbandonFn")]
        public delegate* unmanaged[Cdecl]<void*, void*, void> on_abandon;

        public void* user_context;
    }

    public enum OrtExternalMemoryHandleType
    {
        ORT_EXTERNAL_MEMORY_HANDLE_TYPE_D3D12_RESOURCE = 0,
        ORT_EXTERNAL_MEMORY_HANDLE_TYPE_D3D12_HEAP = 1,
        ORT_EXTERNAL_MEMORY_HANDLE_TYPE_VK_MEMORY_WIN32 = 2,
        ORT_EXTERNAL_MEMORY_HANDLE_TYPE_VK_MEMORY_OPAQUE_FD = 3,
    }

    public unsafe partial struct OrtExternalMemoryDescriptor
    {
        [NativeTypeName("uint32_t")]
        public uint version;

        public OrtExternalMemoryHandleType handle_type;

        public void* native_handle;

        [NativeTypeName("size_t")]
        public nuint size_bytes;

        [NativeTypeName("size_t")]
        public nuint offset_bytes;
    }

    public enum OrtExternalSemaphoreType
    {
        ORT_EXTERNAL_SEMAPHORE_D3D12_FENCE = 0,
        ORT_EXTERNAL_SEMAPHORE_VK_TIMELINE_SEMAPHORE_WIN32 = 1,
        ORT_EXTERNAL_SEMAPHORE_VK_TIMELINE_SEMAPHORE_OPAQUE_FD = 2,
    }

    public unsafe partial struct OrtExternalSemaphoreDescriptor
    {
        [NativeTypeName("uint32_t")]
        public uint version;

        public OrtExternalSemaphoreType type;

        public void* native_handle;
    }

    public enum OrtGraphicsApi
    {
        ORT_GRAPHICS_API_NONE = 0,
        ORT_GRAPHICS_API_D3D12 = 1,
        ORT_GRAPHICS_API_VULKAN = 2,
    }

    public unsafe partial struct OrtGraphicsInteropConfig
    {
        [NativeTypeName("uint32_t")]
        public uint version;

        public OrtGraphicsApi graphics_api;

        public void* command_queue;

        [NativeTypeName("const OrtKeyValuePairs *")]
        public OrtKeyValuePairs* additional_options;
    }

    public unsafe partial struct OrtExternalTensorDescriptor
    {
        [NativeTypeName("uint32_t")]
        public uint version;

        public ONNXTensorElementDataType element_type;

        [NativeTypeName("const int64_t *")]
        public long* shape;

        [NativeTypeName("size_t")]
        public nuint rank;

        [NativeTypeName("size_t")]
        public nuint offset_bytes;
    }

    public enum OrtCompiledModelCompatibility
    {
        OrtCompiledModelCompatibility_EP_NOT_APPLICABLE = 0,
        OrtCompiledModelCompatibility_EP_SUPPORTED_OPTIMAL,
        OrtCompiledModelCompatibility_EP_SUPPORTED_PREFER_RECOMPILATION,
        OrtCompiledModelCompatibility_EP_UNSUPPORTED,
    }

    public unsafe partial struct OrtEnvCreationOptions
    {
        [NativeTypeName("uint32_t")]
        public uint version;

        [NativeTypeName("int32_t")]
        public int logging_severity_level;

        [NativeTypeName("const char *")]
        public sbyte* log_id;

        [NativeTypeName("OrtLoggingFunction")]
        public delegate* unmanaged[Stdcall]<void*, OrtLoggingLevel, sbyte*, sbyte*, sbyte*, sbyte*, void> custom_logging_function;

        public void* custom_logging_param;

        [NativeTypeName("const OrtThreadingOptions *")]
        public OrtThreadingOptions* threading_options;

        [NativeTypeName("const OrtKeyValuePairs *")]
        public OrtKeyValuePairs* config_entries;
    }

    public unsafe partial struct OrtApi
    {
        [NativeTypeName("OrtStatus *(*)(OrtErrorCode, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtErrorCode, sbyte*, OrtStatusHandle> CreateStatus;

        [NativeTypeName("OrtErrorCode (*)(const OrtStatus *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtStatusHandle, OrtErrorCode> GetErrorCode;

        [NativeTypeName("const char *(*)(const OrtStatus *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtStatusHandle, sbyte*> GetErrorMessage;

        [NativeTypeName("OrtStatusPtr (*)(OrtLoggingLevel, const char *, OrtEnv **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtLoggingLevel, sbyte*, OrtEnv**, OrtStatusHandle> CreateEnv;

        [NativeTypeName("OrtStatusPtr (*)(OrtLoggingFunction, void *, OrtLoggingLevel, const char *, OrtEnv **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<delegate* unmanaged[Stdcall]<void*, OrtLoggingLevel, sbyte*, sbyte*, sbyte*, sbyte*, void>, void*, OrtLoggingLevel, sbyte*, OrtEnv**, OrtStatusHandle> CreateEnvWithCustomLogger;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtStatusHandle> EnableTelemetryEvents;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtStatusHandle> DisableTelemetryEvents;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const wchar_t *, const OrtSessionOptions *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, ushort*, OrtSessionOptions*, OrtSession**, OrtStatusHandle> CreateSession;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const void *, size_t, const OrtSessionOptions *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, void*, nuint, OrtSessionOptions*, OrtSession**, OrtStatusHandle> CreateSessionFromArray;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, const OrtRunOptions *, const char *const *, const OrtValue *const *, size_t, const char *const *, size_t, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtRunOptions*, sbyte**, OrtValue**, nuint, sbyte**, nuint, OrtValue**, OrtStatusHandle> Run;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions**, OrtStatusHandle> CreateSessionOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, ushort*, OrtStatusHandle> SetOptimizedModelFilePath;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSessionOptions *, OrtSessionOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtSessionOptions**, OrtStatusHandle> CloneSessionOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, ExecutionMode) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, ExecutionMode, OrtStatusHandle> SetSessionExecutionMode;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, ushort*, OrtStatusHandle> EnableProfiling;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatusHandle> DisableProfiling;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatusHandle> EnableMemPattern;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatusHandle> DisableMemPattern;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatusHandle> EnableCpuMemArena;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatusHandle> DisableCpuMemArena;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, OrtStatusHandle> SetSessionLogId;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, int, OrtStatusHandle> SetSessionLogVerbosityLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, int, OrtStatusHandle> SetSessionLogSeverityLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, GraphOptimizationLevel) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, GraphOptimizationLevel, OrtStatusHandle> SetSessionGraphOptimizationLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, int, OrtStatusHandle> SetIntraOpNumThreads;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, int, OrtStatusHandle> SetInterOpNumThreads;

        [NativeTypeName("OrtStatusPtr (*)(const char *, OrtCustomOpDomain **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, OrtCustomOpDomain**, OrtStatusHandle> CreateCustomOpDomain;

        [NativeTypeName("OrtStatusPtr (*)(OrtCustomOpDomain *, const OrtCustomOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOpDomain*, OrtCustomOp*, OrtStatusHandle> CustomOpDomain_Add;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, OrtCustomOpDomain *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtCustomOpDomain*, OrtStatusHandle> AddCustomOpDomain;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, void**, OrtStatusHandle> RegisterCustomOpsLibrary;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint*, OrtStatusHandle> SessionGetInputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint*, OrtStatusHandle> SessionGetOutputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint*, OrtStatusHandle> SessionGetOverridableInitializerCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint, OrtTypeInfo**, OrtStatusHandle> SessionGetInputTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint, OrtTypeInfo**, OrtStatusHandle> SessionGetOutputTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint, OrtTypeInfo**, OrtStatusHandle> SessionGetOverridableInitializerTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint, OrtAllocator*, sbyte**, OrtStatusHandle> SessionGetInputName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint, OrtAllocator*, sbyte**, OrtStatusHandle> SessionGetOutputName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint, OrtAllocator*, sbyte**, OrtStatusHandle> SessionGetOverridableInitializerName;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions**, OrtStatusHandle> CreateRunOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, int, OrtStatusHandle> RunOptionsSetRunLogVerbosityLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, int, OrtStatusHandle> RunOptionsSetRunLogSeverityLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, sbyte*, OrtStatusHandle> RunOptionsSetRunTag;

        [NativeTypeName("OrtStatusPtr (*)(const OrtRunOptions *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, int*, OrtStatusHandle> RunOptionsGetRunLogVerbosityLevel;

        [NativeTypeName("OrtStatusPtr (*)(const OrtRunOptions *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, int*, OrtStatusHandle> RunOptionsGetRunLogSeverityLevel;

        [NativeTypeName("OrtStatusPtr (*)(const OrtRunOptions *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, sbyte**, OrtStatusHandle> RunOptionsGetRunTag;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, OrtStatusHandle> RunOptionsSetTerminate;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, OrtStatusHandle> RunOptionsUnsetTerminate;

        [NativeTypeName("OrtStatusPtr (*)(OrtAllocator *, const int64_t *, size_t, ONNXTensorElementDataType, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, long*, nuint, ONNXTensorElementDataType, OrtValue**, OrtStatusHandle> CreateTensorAsOrtValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, void *, size_t, const int64_t *, size_t, ONNXTensorElementDataType, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, void*, nuint, long*, nuint, ONNXTensorElementDataType, OrtValue**, OrtStatusHandle> CreateTensorWithDataAsOrtValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, int*, OrtStatusHandle> IsTensor;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, void**, OrtStatusHandle> GetTensorMutableData;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, sbyte**, nuint, OrtStatusHandle> FillStringTensor;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, nuint*, OrtStatusHandle> GetStringTensorDataLength;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, void *, size_t, size_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, void*, nuint, nuint*, nuint, OrtStatusHandle> GetStringTensorContent;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, const OrtTensorTypeAndShapeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, OrtTensorTypeAndShapeInfo**, OrtStatusHandle> CastTypeInfoToTensorInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, enum ONNXType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, ONNXType*, OrtStatusHandle> GetOnnxTypeFromTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorTypeAndShapeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo**, OrtStatusHandle> CreateTensorTypeAndShapeInfo;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorTypeAndShapeInfo *, enum ONNXTensorElementDataType) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, ONNXTensorElementDataType, OrtStatusHandle> SetTensorElementType;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorTypeAndShapeInfo *, const int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, long*, nuint, OrtStatusHandle> SetDimensions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, enum ONNXTensorElementDataType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, ONNXTensorElementDataType*, OrtStatusHandle> GetTensorElementType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, nuint*, OrtStatusHandle> GetDimensionsCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, long*, nuint, OrtStatusHandle> GetDimensions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, const char **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, sbyte**, nuint, OrtStatusHandle> GetSymbolicDimensions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, nuint*, OrtStatusHandle> GetTensorShapeElementCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, OrtTensorTypeAndShapeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtTensorTypeAndShapeInfo**, OrtStatusHandle> GetTensorTypeAndShape;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtTypeInfo**, OrtStatusHandle> GetTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, enum ONNXType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, ONNXType*, OrtStatusHandle> GetValueType;

        [NativeTypeName("OrtStatusPtr (*)(const char *, enum OrtAllocatorType, int, enum OrtMemType, OrtMemoryInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, OrtAllocatorType, int, OrtMemType, OrtMemoryInfo**, OrtStatusHandle> CreateMemoryInfo;

        [NativeTypeName("OrtStatusPtr (*)(enum OrtAllocatorType, enum OrtMemType, OrtMemoryInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocatorType, OrtMemType, OrtMemoryInfo**, OrtStatusHandle> CreateCpuMemoryInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, const OrtMemoryInfo *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, OrtMemoryInfo*, int*, OrtStatusHandle> CompareMemoryInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, sbyte**, OrtStatusHandle> MemoryInfoGetName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, int*, OrtStatusHandle> MemoryInfoGetId;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, OrtMemType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, OrtMemType*, OrtStatusHandle> MemoryInfoGetMemType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, OrtAllocatorType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, OrtAllocatorType*, OrtStatusHandle> MemoryInfoGetType;

        [NativeTypeName("OrtStatusPtr (*)(OrtAllocator *, size_t, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, nuint, void**, OrtStatusHandle> AllocatorAlloc;

        [NativeTypeName("OrtStatusPtr (*)(OrtAllocator *, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, void*, OrtStatusHandle> AllocatorFree;

        [NativeTypeName("OrtStatusPtr (*)(const OrtAllocator *, const struct OrtMemoryInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, OrtMemoryInfo**, OrtStatusHandle> AllocatorGetInfo;

        [NativeTypeName("OrtStatusPtr (*)(OrtAllocator **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator**, OrtStatusHandle> GetAllocatorWithDefaultOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *, int64_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, long, OrtStatusHandle> AddFreeDimensionOverride;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, int, OrtAllocator *, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, int, OrtAllocator*, OrtValue**, OrtStatusHandle> GetValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, nuint*, OrtStatusHandle> GetValueCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *const *, size_t, enum ONNXType, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue**, nuint, ONNXType, OrtValue**, OrtStatusHandle> CreateValue;

        [NativeTypeName("OrtStatusPtr (*)(const char *, const char *, const void *, size_t, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, sbyte*, void*, nuint, OrtValue**, OrtStatusHandle> CreateOpaqueValue;

        [NativeTypeName("OrtStatusPtr (*)(const char *, const char *, const OrtValue *, void *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, sbyte*, OrtValue*, void*, nuint, OrtStatusHandle> GetOpaqueValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, float *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, float*, OrtStatusHandle> KernelInfoGetAttribute_float;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, int64_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, long*, OrtStatusHandle> KernelInfoGetAttribute_int64;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, sbyte*, nuint*, OrtStatusHandle> KernelInfoGetAttribute_string;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, nuint*, OrtStatusHandle> KernelContext_GetInputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, nuint*, OrtStatusHandle> KernelContext_GetOutputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, size_t, const OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, nuint, OrtValue**, OrtStatusHandle> KernelContext_GetInput;

        [NativeTypeName("OrtStatusPtr (*)(OrtKernelContext *, size_t, const int64_t *, size_t, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, nuint, long*, nuint, OrtValue**, OrtStatusHandle> KernelContext_GetOutput;

        [NativeTypeName("void (*)(OrtEnv *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, void> ReleaseEnv;

        [NativeTypeName("void (*)(OrtStatus *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtStatusHandle, void> ReleaseStatus;

        [NativeTypeName("void (*)(OrtMemoryInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, void> ReleaseMemoryInfo;

        [NativeTypeName("void (*)(OrtSession *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, void> ReleaseSession;

        [NativeTypeName("void (*)(OrtValue *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, void> ReleaseValue;

        [NativeTypeName("void (*)(OrtRunOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, void> ReleaseRunOptions;

        [NativeTypeName("void (*)(OrtTypeInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, void> ReleaseTypeInfo;

        [NativeTypeName("void (*)(OrtTensorTypeAndShapeInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, void> ReleaseTensorTypeAndShapeInfo;

        [NativeTypeName("void (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, void> ReleaseSessionOptions;

        [NativeTypeName("void (*)(OrtCustomOpDomain *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOpDomain*, void> ReleaseCustomOpDomain;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, const char **const, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, sbyte**, nuint*, OrtStatusHandle> GetDenotationFromTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, const OrtMapTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, OrtMapTypeInfo**, OrtStatusHandle> CastTypeInfoToMapTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, const OrtSequenceTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, OrtSequenceTypeInfo**, OrtStatusHandle> CastTypeInfoToSequenceTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMapTypeInfo *, enum ONNXTensorElementDataType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMapTypeInfo*, ONNXTensorElementDataType*, OrtStatusHandle> GetMapKeyType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMapTypeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMapTypeInfo*, OrtTypeInfo**, OrtStatusHandle> GetMapValueType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSequenceTypeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSequenceTypeInfo*, OrtTypeInfo**, OrtStatusHandle> GetSequenceElementType;

        [NativeTypeName("void (*)(OrtMapTypeInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMapTypeInfo*, void> ReleaseMapTypeInfo;

        [NativeTypeName("void (*)(OrtSequenceTypeInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSequenceTypeInfo*, void> ReleaseSequenceTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtAllocator*, sbyte**, OrtStatusHandle> SessionEndProfiling;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, OrtModelMetadata **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtModelMetadata**, OrtStatusHandle> SessionGetModelMetadata;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte**, OrtStatusHandle> ModelMetadataGetProducerName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte**, OrtStatusHandle> ModelMetadataGetGraphName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte**, OrtStatusHandle> ModelMetadataGetDomain;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte**, OrtStatusHandle> ModelMetadataGetDescription;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, const char *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte*, sbyte**, OrtStatusHandle> ModelMetadataLookupCustomMetadataMap;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, int64_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, long*, OrtStatusHandle> ModelMetadataGetVersion;

        [NativeTypeName("void (*)(OrtModelMetadata *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, void> ReleaseModelMetadata;

        [NativeTypeName("OrtStatusPtr (*)(OrtLoggingLevel, const char *, const OrtThreadingOptions *, OrtEnv **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtLoggingLevel, sbyte*, OrtThreadingOptions*, OrtEnv**, OrtStatusHandle> CreateEnvWithGlobalThreadPools;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatusHandle> DisablePerSessionThreads;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions**, OrtStatusHandle> CreateThreadingOptions;

        [NativeTypeName("void (*)(OrtThreadingOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, void> ReleaseThreadingOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, char ***, int64_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte***, long*, OrtStatusHandle> ModelMetadataGetCustomMetadataMapKeys;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *, int64_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, long, OrtStatusHandle> AddFreeDimensionOverrideByName;

        [NativeTypeName("OrtStatusPtr (*)(char ***, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte***, int*, OrtStatusHandle> GetAvailableProviders;

        [NativeTypeName("OrtStatusPtr (*)(char **, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte**, int, OrtStatusHandle> ReleaseAvailableProviders;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, size_t, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, nuint, nuint*, OrtStatusHandle> GetStringTensorElementLength;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, size_t, size_t, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, nuint, nuint, void*, OrtStatusHandle> GetStringTensorElement;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const char *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, sbyte*, nuint, OrtStatusHandle> FillStringTensorElement;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, sbyte*, OrtStatusHandle> AddSessionConfigEntry;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const OrtMemoryInfo *, OrtAllocator **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtMemoryInfo*, OrtAllocator**, OrtStatusHandle> CreateAllocator;

        [NativeTypeName("void (*)(OrtAllocator *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, void> ReleaseAllocator;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, const OrtRunOptions *, const OrtIoBinding *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtRunOptions*, OrtIoBinding*, OrtStatusHandle> RunWithBinding;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, OrtIoBinding **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtIoBinding**, OrtStatusHandle> CreateIoBinding;

        [NativeTypeName("void (*)(OrtIoBinding *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, void> ReleaseIoBinding;

        [NativeTypeName("OrtStatusPtr (*)(OrtIoBinding *, const char *, const OrtValue *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, sbyte*, OrtValue*, OrtStatusHandle> BindInput;

        [NativeTypeName("OrtStatusPtr (*)(OrtIoBinding *, const char *, const OrtValue *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, sbyte*, OrtValue*, OrtStatusHandle> BindOutput;

        [NativeTypeName("OrtStatusPtr (*)(OrtIoBinding *, const char *, const OrtMemoryInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, sbyte*, OrtMemoryInfo*, OrtStatusHandle> BindOutputToDevice;

        [NativeTypeName("OrtStatusPtr (*)(const OrtIoBinding *, OrtAllocator *, char **, size_t **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, OrtAllocator*, sbyte**, nuint**, nuint*, OrtStatusHandle> GetBoundOutputNames;

        [NativeTypeName("OrtStatusPtr (*)(const OrtIoBinding *, OrtAllocator *, OrtValue ***, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, OrtAllocator*, OrtValue***, nuint*, OrtStatusHandle> GetBoundOutputValues;

        [NativeTypeName("void (*)(OrtIoBinding *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, void> ClearBoundInputs;

        [NativeTypeName("void (*)(OrtIoBinding *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, void> ClearBoundOutputs;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const int64_t *, size_t, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, long*, nuint, void**, OrtStatusHandle> TensorAt;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const OrtMemoryInfo *, const OrtArenaCfg *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtMemoryInfo*, OrtArenaCfg*, OrtStatusHandle> CreateAndRegisterAllocator;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, OrtLanguageProjection) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtLanguageProjection, OrtStatusHandle> SetLanguageProjection;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, uint64_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, ulong*, OrtStatusHandle> SessionGetProfilingStartTimeNs;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, int, OrtStatusHandle> SetGlobalIntraOpNumThreads;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, int, OrtStatusHandle> SetGlobalInterOpNumThreads;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, int, OrtStatusHandle> SetGlobalSpinControl;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *, const OrtValue *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, OrtValue*, OrtStatusHandle> AddInitializer;

        [NativeTypeName("OrtStatusPtr (*)(OrtLoggingFunction, void *, OrtLoggingLevel, const char *, const struct OrtThreadingOptions *, OrtEnv **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<delegate* unmanaged[Stdcall]<void*, OrtLoggingLevel, sbyte*, sbyte*, sbyte*, sbyte*, void>, void*, OrtLoggingLevel, sbyte*, OrtThreadingOptions*, OrtEnv**, OrtStatusHandle> CreateEnvWithCustomLoggerAndGlobalThreadPools;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtCUDAProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtCUDAProviderOptions*, OrtStatusHandle> SessionOptionsAppendExecutionProvider_CUDA;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtROCMProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtROCMProviderOptions*, OrtStatusHandle> SessionOptionsAppendExecutionProvider_ROCM;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtOpenVINOProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtOpenVINOProviderOptions*, OrtStatusHandle> SessionOptionsAppendExecutionProvider_OpenVINO;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, OrtStatusHandle> SetGlobalDenormalAsZero;

        [NativeTypeName("OrtStatusPtr (*)(size_t, int, int, int, OrtArenaCfg **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<nuint, int, int, int, OrtArenaCfg**, OrtStatusHandle> CreateArenaCfg;

        [NativeTypeName("void (*)(OrtArenaCfg *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtArenaCfg*, void> ReleaseArenaCfg;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte**, OrtStatusHandle> ModelMetadataGetGraphDescription;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtTensorRTProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtTensorRTProviderOptions*, OrtStatusHandle> SessionOptionsAppendExecutionProvider_TensorRT;

        [NativeTypeName("OrtStatusPtr (*)(int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<int, OrtStatusHandle> SetCurrentGpuDeviceId;

        [NativeTypeName("OrtStatusPtr (*)(int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<int*, OrtStatusHandle> GetCurrentGpuDeviceId;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, float *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, float*, nuint*, OrtStatusHandle> KernelInfoGetAttributeArray_float;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, int64_t *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, long*, nuint*, OrtStatusHandle> KernelInfoGetAttributeArray_int64;

        [NativeTypeName("OrtStatusPtr (*)(const char *const *, const size_t *, size_t, OrtArenaCfg **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte**, nuint*, nuint, OrtArenaCfg**, OrtStatusHandle> CreateArenaCfgV2;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *, const char *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, sbyte*, sbyte*, OrtStatusHandle> AddRunConfigEntry;

        [NativeTypeName("OrtStatusPtr (*)(OrtPrepackedWeightsContainer **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtPrepackedWeightsContainer**, OrtStatusHandle> CreatePrepackedWeightsContainer;

        [NativeTypeName("void (*)(OrtPrepackedWeightsContainer *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtPrepackedWeightsContainer*, void> ReleasePrepackedWeightsContainer;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const wchar_t *, const OrtSessionOptions *, OrtPrepackedWeightsContainer *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, ushort*, OrtSessionOptions*, OrtPrepackedWeightsContainer*, OrtSession**, OrtStatusHandle> CreateSessionWithPrepackedWeightsContainer;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const void *, size_t, const OrtSessionOptions *, OrtPrepackedWeightsContainer *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, void*, nuint, OrtSessionOptions*, OrtPrepackedWeightsContainer*, OrtSession**, OrtStatusHandle> CreateSessionFromArrayWithPrepackedWeightsContainer;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtTensorRTProviderOptionsV2 *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtTensorRTProviderOptionsV2*, OrtStatusHandle> SessionOptionsAppendExecutionProvider_TensorRT_V2;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorRTProviderOptionsV2 **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorRTProviderOptionsV2**, OrtStatusHandle> CreateTensorRTProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorRTProviderOptionsV2 *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorRTProviderOptionsV2*, sbyte**, sbyte**, nuint, OrtStatusHandle> UpdateTensorRTProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorRTProviderOptionsV2 *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorRTProviderOptionsV2*, OrtAllocator*, sbyte**, OrtStatusHandle> GetTensorRTProviderOptionsAsString;

        [NativeTypeName("void (*)(OrtTensorRTProviderOptionsV2 *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorRTProviderOptionsV2*, void> ReleaseTensorRTProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatusHandle> EnableOrtCustomOps;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, OrtAllocator *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtAllocator*, OrtStatusHandle> RegisterAllocator;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const OrtMemoryInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtMemoryInfo*, OrtStatusHandle> UnregisterAllocator;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, int*, OrtStatusHandle> IsSparseTensor;

        [NativeTypeName("OrtStatusPtr (*)(OrtAllocator *, const int64_t *, size_t, ONNXTensorElementDataType, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, long*, nuint, ONNXTensorElementDataType, OrtValue**, OrtStatusHandle> CreateSparseTensorAsOrtValue;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const OrtMemoryInfo *, const int64_t *, size_t, const void *, const int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtMemoryInfo*, long*, nuint, void*, long*, nuint, OrtStatusHandle> FillSparseTensorCoo;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const OrtMemoryInfo *, const int64_t *, size_t, const void *, const int64_t *, size_t, const int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtMemoryInfo*, long*, nuint, void*, long*, nuint, long*, nuint, OrtStatusHandle> FillSparseTensorCsr;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const OrtMemoryInfo *, const int64_t *, size_t, const void *, const int64_t *, size_t, const int32_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtMemoryInfo*, long*, nuint, void*, long*, nuint, int*, OrtStatusHandle> FillSparseTensorBlockSparse;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, void *, const int64_t *, size_t, const int64_t *, size_t, ONNXTensorElementDataType, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, void*, long*, nuint, long*, nuint, ONNXTensorElementDataType, OrtValue**, OrtStatusHandle> CreateSparseTensorWithValuesAsOrtValue;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, long*, nuint, OrtStatusHandle> UseCooIndices;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, int64_t *, size_t, int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, long*, nuint, long*, nuint, OrtStatusHandle> UseCsrIndices;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const int64_t *, size_t, int32_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, long*, nuint, int*, OrtStatusHandle> UseBlockSparseIndices;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, enum OrtSparseFormat *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtSparseFormat*, OrtStatusHandle> GetSparseTensorFormat;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, OrtTensorTypeAndShapeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtTensorTypeAndShapeInfo**, OrtStatusHandle> GetSparseTensorValuesTypeAndShape;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, const void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, void**, OrtStatusHandle> GetSparseTensorValues;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, enum OrtSparseIndicesFormat, OrtTensorTypeAndShapeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtSparseIndicesFormat, OrtTensorTypeAndShapeInfo**, OrtStatusHandle> GetSparseTensorIndicesTypeShape;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, enum OrtSparseIndicesFormat, size_t *, const void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtSparseIndicesFormat, nuint*, void**, OrtStatusHandle> GetSparseTensorIndices;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, int*, OrtStatusHandle> HasValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, void**, OrtStatusHandle> KernelContext_GetGPUComputeStream;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, const OrtMemoryInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtMemoryInfo**, OrtStatusHandle> GetTensorMemoryInfo;

        [NativeTypeName("OrtStatusPtr (*)(const char *, uint32_t, const void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, uint, void**, OrtStatusHandle> GetExecutionProviderApi;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, OrtCustomCreateThreadFn) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, delegate* unmanaged[Cdecl]<void*, delegate* unmanaged[Cdecl]<void*, void>, void*, OrtCustomHandleType*>, OrtStatusHandle> SessionOptionsSetCustomCreateThreadFn;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, void*, OrtStatusHandle> SessionOptionsSetCustomThreadCreationOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, OrtCustomJoinThreadFn) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, delegate* unmanaged[Cdecl]<OrtCustomHandleType*, void>, OrtStatusHandle> SessionOptionsSetCustomJoinThreadFn;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, OrtCustomCreateThreadFn) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, delegate* unmanaged[Cdecl]<void*, delegate* unmanaged[Cdecl]<void*, void>, void*, OrtCustomHandleType*>, OrtStatusHandle> SetGlobalCustomCreateThreadFn;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, void*, OrtStatusHandle> SetGlobalCustomThreadCreationOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, OrtCustomJoinThreadFn) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, delegate* unmanaged[Cdecl]<OrtCustomHandleType*, void>, OrtStatusHandle> SetGlobalCustomJoinThreadFn;

        [NativeTypeName("OrtStatusPtr (*)(OrtIoBinding *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, OrtStatusHandle> SynchronizeBoundInputs;

        [NativeTypeName("OrtStatusPtr (*)(OrtIoBinding *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, OrtStatusHandle> SynchronizeBoundOutputs;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtCUDAProviderOptionsV2 *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtCUDAProviderOptionsV2*, OrtStatusHandle> SessionOptionsAppendExecutionProvider_CUDA_V2;

        [NativeTypeName("OrtStatusPtr (*)(OrtCUDAProviderOptionsV2 **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCUDAProviderOptionsV2**, OrtStatusHandle> CreateCUDAProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtCUDAProviderOptionsV2 *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCUDAProviderOptionsV2*, sbyte**, sbyte**, nuint, OrtStatusHandle> UpdateCUDAProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtCUDAProviderOptionsV2 *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCUDAProviderOptionsV2*, OrtAllocator*, sbyte**, OrtStatusHandle> GetCUDAProviderOptionsAsString;

        [NativeTypeName("void (*)(OrtCUDAProviderOptionsV2 *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCUDAProviderOptionsV2*, void> ReleaseCUDAProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtMIGraphXProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtMIGraphXProviderOptions*, OrtStatusHandle> SessionOptionsAppendExecutionProvider_MIGraphX;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *const *, const OrtValue *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte**, OrtValue**, nuint, OrtStatusHandle> AddExternalInitializers;

        [NativeTypeName("OrtStatusPtr (*)(const char *, const void *, int, OrtOpAttrType, OrtOpAttr **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, void*, int, OrtOpAttrType, OrtOpAttr**, OrtStatusHandle> CreateOpAttr;

        [NativeTypeName("void (*)(OrtOpAttr *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOpAttr*, void> ReleaseOpAttr;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, const char *, int, const char **, const ONNXTensorElementDataType *, int, const OrtOpAttr *const *, int, int, int, OrtOp **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, sbyte*, int, sbyte**, ONNXTensorElementDataType*, int, OrtOpAttr**, int, int, int, OrtOp**, OrtStatusHandle> CreateOp;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, const OrtOp *, const OrtValue *const *, int, OrtValue *const *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, OrtOp*, OrtValue**, int, OrtValue**, int, OrtStatusHandle> InvokeOp;

        [NativeTypeName("void (*)(OrtOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOp*, void> ReleaseOp;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, sbyte**, sbyte**, nuint, OrtStatusHandle> SessionOptionsAppendExecutionProvider;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, OrtKernelInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, OrtKernelInfo**, OrtStatusHandle> CopyKernelInfo;

        [NativeTypeName("void (*)(OrtKernelInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, void> ReleaseKernelInfo;

        [NativeTypeName("const OrtTrainingApi *(*)(uint32_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<uint, OrtTrainingApi*> GetTrainingApi;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtCANNProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtCANNProviderOptions*, OrtStatusHandle> SessionOptionsAppendExecutionProvider_CANN;

        [NativeTypeName("OrtStatusPtr (*)(OrtCANNProviderOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCANNProviderOptions**, OrtStatusHandle> CreateCANNProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtCANNProviderOptions *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCANNProviderOptions*, sbyte**, sbyte**, nuint, OrtStatusHandle> UpdateCANNProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtCANNProviderOptions *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCANNProviderOptions*, OrtAllocator*, sbyte**, OrtStatusHandle> GetCANNProviderOptionsAsString;

        [NativeTypeName("void (*)(OrtCANNProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCANNProviderOptions*, void> ReleaseCANNProviderOptions;

        [NativeTypeName("void (*)(const OrtMemoryInfo *, OrtMemoryInfoDeviceType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, OrtMemoryInfoDeviceType*, void> MemoryInfoGetDeviceType;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, OrtLoggingLevel) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtLoggingLevel, OrtStatusHandle> UpdateEnvWithCustomLogLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, sbyte*, OrtStatusHandle> SetGlobalIntraOpThreadAffinity;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, ushort*, OrtStatusHandle> RegisterCustomOpsLibrary_V2;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, OrtStatusHandle> RegisterCustomOpsUsingFunction;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint*, OrtStatusHandle> KernelInfo_GetInputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint*, OrtStatusHandle> KernelInfo_GetOutputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint, sbyte*, nuint*, OrtStatusHandle> KernelInfo_GetInputName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint, sbyte*, nuint*, OrtStatusHandle> KernelInfo_GetOutputName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint, OrtTypeInfo**, OrtStatusHandle> KernelInfo_GetInputTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint, OrtTypeInfo**, OrtStatusHandle> KernelInfo_GetOutputTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, OrtAllocator *, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, OrtAllocator*, OrtValue**, OrtStatusHandle> KernelInfoGetAttribute_tensor;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSessionOptions *, const char *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, int*, OrtStatusHandle> HasSessionConfigEntry;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSessionOptions *, const char *, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, sbyte*, nuint*, OrtStatusHandle> GetSessionConfigEntry;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtDnnlProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtDnnlProviderOptions*, OrtStatusHandle> SessionOptionsAppendExecutionProvider_Dnnl;

        [NativeTypeName("OrtStatusPtr (*)(OrtDnnlProviderOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDnnlProviderOptions**, OrtStatusHandle> CreateDnnlProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtDnnlProviderOptions *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDnnlProviderOptions*, sbyte**, sbyte**, nuint, OrtStatusHandle> UpdateDnnlProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtDnnlProviderOptions *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDnnlProviderOptions*, OrtAllocator*, sbyte**, OrtStatusHandle> GetDnnlProviderOptionsAsString;

        [NativeTypeName("void (*)(OrtDnnlProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDnnlProviderOptions*, void> ReleaseDnnlProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, nuint*, OrtStatusHandle> KernelInfo_GetNodeName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const OrtLogger **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, OrtLogger**, OrtStatusHandle> KernelInfo_GetLogger;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, const OrtLogger **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, OrtLogger**, OrtStatusHandle> KernelContext_GetLogger;

        [NativeTypeName("OrtStatusPtr (*)(const OrtLogger *, OrtLoggingLevel, const char *, const wchar_t *, int, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtLogger*, OrtLoggingLevel, sbyte*, ushort*, int, sbyte*, OrtStatusHandle> Logger_LogMessage;

        [NativeTypeName("OrtStatusPtr (*)(const OrtLogger *, OrtLoggingLevel *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtLogger*, OrtLoggingLevel*, OrtStatusHandle> Logger_GetLoggingSeverityLevel;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t, int *, const OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint, int*, OrtValue**, OrtStatusHandle> KernelInfoGetConstantInput_tensor;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, const OrtOptionalTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, OrtOptionalTypeInfo**, OrtStatusHandle> CastTypeInfoToOptionalTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtOptionalTypeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOptionalTypeInfo*, OrtTypeInfo**, OrtStatusHandle> GetOptionalContainedTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, size_t, size_t, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, nuint, nuint, sbyte**, OrtStatusHandle> GetResizedStringTensorElementBuffer;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, const OrtMemoryInfo *, OrtAllocator **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, OrtMemoryInfo*, OrtAllocator**, OrtStatusHandle> KernelContext_GetAllocator;

        [NativeTypeName("const char *(*)(void) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*> GetBuildInfoString;

        [NativeTypeName("OrtStatusPtr (*)(OrtROCMProviderOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtROCMProviderOptions**, OrtStatusHandle> CreateROCMProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtROCMProviderOptions *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtROCMProviderOptions*, sbyte**, sbyte**, nuint, OrtStatusHandle> UpdateROCMProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtROCMProviderOptions *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtROCMProviderOptions*, OrtAllocator*, sbyte**, OrtStatusHandle> GetROCMProviderOptionsAsString;

        [NativeTypeName("void (*)(OrtROCMProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtROCMProviderOptions*, void> ReleaseROCMProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const char *, const OrtMemoryInfo *, const OrtArenaCfg *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, sbyte*, OrtMemoryInfo*, OrtArenaCfg*, sbyte**, sbyte**, nuint, OrtStatusHandle> CreateAndRegisterAllocatorV2;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, const OrtRunOptions *, const char *const *, const OrtValue *const *, size_t, const char *const *, size_t, OrtValue **, RunAsyncCallbackFn, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtRunOptions*, sbyte**, OrtValue**, nuint, sbyte**, nuint, OrtValue**, delegate* unmanaged[Cdecl]<void*, OrtValue**, nuint, void*, void>, void*, OrtStatusHandle> RunAsync;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorRTProviderOptionsV2 *, const char *, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorRTProviderOptionsV2*, sbyte*, void*, OrtStatusHandle> UpdateTensorRTProviderOptionsWithValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorRTProviderOptionsV2 *, const char *, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorRTProviderOptionsV2*, sbyte*, void**, OrtStatusHandle> GetTensorRTProviderOptionsByName;

        [NativeTypeName("OrtStatusPtr (*)(OrtCUDAProviderOptionsV2 *, const char *, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCUDAProviderOptionsV2*, sbyte*, void*, OrtStatusHandle> UpdateCUDAProviderOptionsWithValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtCUDAProviderOptionsV2 *, const char *, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCUDAProviderOptionsV2*, sbyte*, void**, OrtStatusHandle> GetCUDAProviderOptionsByName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, int, int, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, int, int, void**, OrtStatusHandle> KernelContext_GetResource;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, OrtLoggingFunction, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, delegate* unmanaged[Stdcall]<void*, OrtLoggingLevel, sbyte*, sbyte*, sbyte*, sbyte*, void>, void*, OrtStatusHandle> SetUserLoggingFunction;

        [NativeTypeName("OrtStatusPtr (*)(const OrtShapeInferContext *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtShapeInferContext*, nuint*, OrtStatusHandle> ShapeInferContext_GetInputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtShapeInferContext *, size_t, OrtTensorTypeAndShapeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtShapeInferContext*, nuint, OrtTensorTypeAndShapeInfo**, OrtStatusHandle> ShapeInferContext_GetInputTypeShape;

        [NativeTypeName("OrtStatusPtr (*)(const OrtShapeInferContext *, const char *, const OrtOpAttr **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtShapeInferContext*, sbyte*, OrtOpAttr**, OrtStatusHandle> ShapeInferContext_GetAttribute;

        [NativeTypeName("OrtStatusPtr (*)(const OrtShapeInferContext *, size_t, const OrtTensorTypeAndShapeInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtShapeInferContext*, nuint, OrtTensorTypeAndShapeInfo*, OrtStatusHandle> ShapeInferContext_SetOutputTypeShape;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorTypeAndShapeInfo *, const char **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, sbyte**, nuint, OrtStatusHandle> SetSymbolicDimensions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtOpAttr *, OrtOpAttrType, void *, size_t, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOpAttr*, OrtOpAttrType, void*, nuint, nuint*, OrtStatusHandle> ReadOpAttr;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, _Bool) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, byte, OrtStatusHandle> SetDeterministicCompute;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, void (*)(void *, size_t), size_t, size_t, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, delegate* unmanaged[Cdecl]<void*, nuint, void>, nuint, nuint, void*, OrtStatusHandle> KernelContext_ParallelFor;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte**, sbyte**, nuint, OrtStatusHandle> SessionOptionsAppendExecutionProvider_OpenVINO_V2;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte**, sbyte**, nuint, OrtStatusHandle> SessionOptionsAppendExecutionProvider_VitisAI;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, const OrtMemoryInfo *, size_t, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, OrtMemoryInfo*, nuint, void**, OrtStatusHandle> KernelContext_GetScratchBuffer;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, OrtMemType, OrtAllocator **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, OrtMemType, OrtAllocator**, OrtStatusHandle> KernelInfoGetAllocator;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const wchar_t *const *, char *const *, const size_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, ushort**, sbyte**, nuint*, nuint, OrtStatusHandle> AddExternalInitializersFromFilesInMemory;

        [NativeTypeName("OrtStatusPtr (*)(const wchar_t *, OrtAllocator *, OrtLoraAdapter **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<ushort*, OrtAllocator*, OrtLoraAdapter**, OrtStatusHandle> CreateLoraAdapter;

        [NativeTypeName("OrtStatusPtr (*)(const void *, size_t, OrtAllocator *, OrtLoraAdapter **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<void*, nuint, OrtAllocator*, OrtLoraAdapter**, OrtStatusHandle> CreateLoraAdapterFromArray;

        [NativeTypeName("void (*)(OrtLoraAdapter *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtLoraAdapter*, void> ReleaseLoraAdapter;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *, const OrtLoraAdapter *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, OrtLoraAdapter*, OrtStatusHandle> RunOptionsAddActiveLoraAdapter;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, sbyte**, sbyte**, nuint, OrtStatusHandle> SetEpDynamicOptions;

        [NativeTypeName("void (*)(OrtValueInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, void> ReleaseValueInfo;

        [NativeTypeName("void (*)(OrtNode *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, void> ReleaseNode;

        [NativeTypeName("void (*)(OrtGraph *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, void> ReleaseGraph;

        [NativeTypeName("void (*)(OrtModel *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModel*, void> ReleaseModel;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, sbyte**, OrtStatusHandle> GetValueInfoName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, const OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, OrtTypeInfo**, OrtStatusHandle> GetValueInfoTypeInfo;

        [NativeTypeName("const OrtModelEditorApi *(*)(void) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelEditorApi*> GetModelEditorApi;

        [NativeTypeName("OrtStatusPtr (*)(OrtAllocator *, void *, size_t, const int64_t *, size_t, ONNXTensorElementDataType, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, void*, nuint, long*, nuint, ONNXTensorElementDataType, OrtValue**, OrtStatusHandle> CreateTensorWithDataAndDeleterAsOrtValue;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, _Bool) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, byte, OrtStatusHandle> SessionOptionsSetLoadCancellationFlag;

        [NativeTypeName("const OrtCompileApi *(*)(void) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCompileApi*> GetCompileApi;

        [NativeTypeName("void (*)(OrtKeyValuePairs **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKeyValuePairs**, void> CreateKeyValuePairs;

        [NativeTypeName("void (*)(OrtKeyValuePairs *, const char *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKeyValuePairs*, sbyte*, sbyte*, void> AddKeyValuePair;

        [NativeTypeName("const char *(*)(const OrtKeyValuePairs *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKeyValuePairs*, sbyte*, sbyte*> GetKeyValue;

        [NativeTypeName("void (*)(const OrtKeyValuePairs *, const char *const **, const char *const **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKeyValuePairs*, sbyte***, sbyte***, nuint*, void> GetKeyValuePairs;

        [NativeTypeName("void (*)(OrtKeyValuePairs *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKeyValuePairs*, sbyte*, void> RemoveKeyValuePair;

        [NativeTypeName("void (*)(OrtKeyValuePairs *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKeyValuePairs*, void> ReleaseKeyValuePairs;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const char *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, sbyte*, ushort*, OrtStatusHandle> RegisterExecutionProviderLibrary;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, sbyte*, OrtStatusHandle> UnregisterExecutionProviderLibrary;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const OrtEpDevice *const **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtEpDevice***, nuint*, OrtStatusHandle> GetEpDevices;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, OrtEnv *, const OrtEpDevice *const *, size_t, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtEnv*, OrtEpDevice**, nuint, sbyte**, sbyte**, nuint, OrtStatusHandle> SessionOptionsAppendExecutionProvider_V2;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, OrtExecutionProviderDevicePolicy) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtExecutionProviderDevicePolicy, OrtStatusHandle> SessionOptionsSetEpSelectionPolicy;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, EpSelectionDelegate, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, delegate* unmanaged[Stdcall]<OrtEpDevice**, nuint, OrtKeyValuePairs*, OrtKeyValuePairs*, OrtEpDevice**, nuint, nuint*, void*, void*>, void*, OrtStatusHandle> SessionOptionsSetEpSelectionPolicyDelegate;

        [NativeTypeName("OrtHardwareDeviceType (*)(const OrtHardwareDevice *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtHardwareDevice*, OrtHardwareDeviceType> HardwareDevice_Type;

        [NativeTypeName("uint32_t (*)(const OrtHardwareDevice *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtHardwareDevice*, uint> HardwareDevice_VendorId;

        [NativeTypeName("const char *(*)(const OrtHardwareDevice *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtHardwareDevice*, sbyte*> HardwareDevice_Vendor;

        [NativeTypeName("uint32_t (*)(const OrtHardwareDevice *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtHardwareDevice*, uint> HardwareDevice_DeviceId;

        [NativeTypeName("const OrtKeyValuePairs *(*)(const OrtHardwareDevice *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtHardwareDevice*, OrtKeyValuePairs*> HardwareDevice_Metadata;

        [NativeTypeName("const char *(*)(const OrtEpDevice *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, sbyte*> EpDevice_EpName;

        [NativeTypeName("const char *(*)(const OrtEpDevice *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, sbyte*> EpDevice_EpVendor;

        [NativeTypeName("const OrtKeyValuePairs *(*)(const OrtEpDevice *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, OrtKeyValuePairs*> EpDevice_EpMetadata;

        [NativeTypeName("const OrtKeyValuePairs *(*)(const OrtEpDevice *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, OrtKeyValuePairs*> EpDevice_EpOptions;

        [NativeTypeName("const OrtHardwareDevice *(*)(const OrtEpDevice *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, OrtHardwareDevice*> EpDevice_Device;

        [NativeTypeName("const OrtEpApi *(*)(void) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpApi*> GetEpApi;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, nuint*, OrtStatusHandle> GetTensorSizeInBytes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtAllocator *, OrtKeyValuePairs **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, OrtKeyValuePairs**, OrtStatusHandle> AllocatorGetStats;

        [NativeTypeName("OrtStatusPtr (*)(const char *, enum OrtMemoryInfoDeviceType, uint32_t, int32_t, enum OrtDeviceMemoryType, size_t, enum OrtAllocatorType, OrtMemoryInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, OrtMemoryInfoDeviceType, uint, int, OrtDeviceMemoryType, nuint, OrtAllocatorType, OrtMemoryInfo**, OrtStatusHandle> CreateMemoryInfo_V2;

        [NativeTypeName("OrtDeviceMemoryType (*)(const OrtMemoryInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, OrtDeviceMemoryType> MemoryInfoGetDeviceMemType;

        [NativeTypeName("uint32_t (*)(const OrtMemoryInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, uint> MemoryInfoGetVendorId;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, const OrtNode **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, OrtNode**, nuint*, OrtStatusHandle> ValueInfo_GetValueProducer;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, nuint*, OrtStatusHandle> ValueInfo_GetValueNumConsumers;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, const OrtNode **, int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, OrtNode**, long*, nuint, OrtStatusHandle> ValueInfo_GetValueConsumers;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, const OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, OrtValue**, OrtStatusHandle> ValueInfo_GetInitializerValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, OrtExternalInitializerInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, OrtExternalInitializerInfo**, OrtStatusHandle> ValueInfo_GetExternalInitializerInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, bool*, OrtStatusHandle> ValueInfo_IsRequiredGraphInput;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, bool*, OrtStatusHandle> ValueInfo_IsOptionalGraphInput;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, bool*, OrtStatusHandle> ValueInfo_IsGraphOutput;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, bool*, OrtStatusHandle> ValueInfo_IsConstantInitializer;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, bool*, OrtStatusHandle> ValueInfo_IsFromOuterScope;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, sbyte**, OrtStatusHandle> Graph_GetName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const wchar_t **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, ushort**, OrtStatusHandle> Graph_GetModelPath;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, int64_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, long*, OrtStatusHandle> Graph_GetOnnxIRVersion;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, nuint*, OrtStatusHandle> Graph_GetNumOperatorSets;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const char **, int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, sbyte**, long*, nuint, OrtStatusHandle> Graph_GetOperatorSets;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, nuint*, OrtStatusHandle> Graph_GetNumInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtValueInfo**, nuint, OrtStatusHandle> Graph_GetInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, nuint*, OrtStatusHandle> Graph_GetNumOutputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtValueInfo**, nuint, OrtStatusHandle> Graph_GetOutputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, nuint*, OrtStatusHandle> Graph_GetNumInitializers;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtValueInfo**, nuint, OrtStatusHandle> Graph_GetInitializers;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, nuint*, OrtStatusHandle> Graph_GetNumNodes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const OrtNode **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtNode**, nuint, OrtStatusHandle> Graph_GetNodes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const OrtNode **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtNode**, OrtStatusHandle> Graph_GetParentNode;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const OrtNode **, size_t, OrtGraph **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtNode**, nuint, OrtGraph**, OrtStatusHandle> Graph_GetGraphView;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, nuint*, OrtStatusHandle> Node_GetId;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, sbyte**, OrtStatusHandle> Node_GetName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, sbyte**, OrtStatusHandle> Node_GetOperatorType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, sbyte**, OrtStatusHandle> Node_GetDomain;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, int*, OrtStatusHandle> Node_GetSinceVersion;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, nuint*, OrtStatusHandle> Node_GetNumInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, OrtValueInfo**, nuint, OrtStatusHandle> Node_GetInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, nuint*, OrtStatusHandle> Node_GetNumOutputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, OrtValueInfo**, nuint, OrtStatusHandle> Node_GetOutputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, nuint*, OrtStatusHandle> Node_GetNumImplicitInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, OrtValueInfo**, nuint, OrtStatusHandle> Node_GetImplicitInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, nuint*, OrtStatusHandle> Node_GetNumAttributes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const OrtOpAttr **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, OrtOpAttr**, nuint, OrtStatusHandle> Node_GetAttributes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const char *, const OrtOpAttr **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, sbyte*, OrtOpAttr**, OrtStatusHandle> Node_GetAttributeByName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtOpAttr *, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOpAttr*, OrtValue**, OrtStatusHandle> OpAttr_GetTensorAttributeAsOrtValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtOpAttr *, OrtOpAttrType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOpAttr*, OrtOpAttrType*, OrtStatusHandle> OpAttr_GetType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtOpAttr *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOpAttr*, sbyte**, OrtStatusHandle> OpAttr_GetName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, nuint*, OrtStatusHandle> Node_GetNumSubgraphs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const OrtGraph **, size_t, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, OrtGraph**, nuint, sbyte**, OrtStatusHandle> Node_GetSubgraphs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const OrtGraph **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, OrtGraph**, OrtStatusHandle> Node_GetGraph;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, sbyte**, OrtStatusHandle> Node_GetEpName;

        [NativeTypeName("void (*)(OrtExternalInitializerInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalInitializerInfo*, void> ReleaseExternalInitializerInfo;

        [NativeTypeName("const wchar_t *(*)(const OrtExternalInitializerInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalInitializerInfo*, ushort*> ExternalInitializerInfo_GetFilePath;

        [NativeTypeName("int64_t (*)(const OrtExternalInitializerInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalInitializerInfo*, long> ExternalInitializerInfo_GetFileOffset;

        [NativeTypeName("size_t (*)(const OrtExternalInitializerInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalInitializerInfo*, nuint> ExternalInitializerInfo_GetByteSize;

        [NativeTypeName("const char *(*)(const OrtRunOptions *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, sbyte*, sbyte*> GetRunConfigEntry;

        [NativeTypeName("const OrtMemoryInfo *(*)(const OrtEpDevice *, OrtDeviceMemoryType) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, OrtDeviceMemoryType, OrtMemoryInfo*> EpDevice_MemoryInfo;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const OrtEpDevice *, OrtDeviceMemoryType, OrtAllocatorType, const OrtKeyValuePairs *, OrtAllocator **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtEpDevice*, OrtDeviceMemoryType, OrtAllocatorType, OrtKeyValuePairs*, OrtAllocator**, OrtStatusHandle> CreateSharedAllocator;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const OrtMemoryInfo *, OrtAllocator **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtMemoryInfo*, OrtAllocator**, OrtStatusHandle> GetSharedAllocator;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const OrtEpDevice *, OrtDeviceMemoryType) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtEpDevice*, OrtDeviceMemoryType, OrtStatusHandle> ReleaseSharedAllocator;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, const void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, void**, OrtStatusHandle> GetTensorData;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSessionOptions *, OrtKeyValuePairs **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtKeyValuePairs**, OrtStatusHandle> GetSessionOptionsConfigEntries;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const OrtMemoryInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtMemoryInfo**, nuint, OrtStatusHandle> SessionGetMemoryInfoForInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const OrtMemoryInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtMemoryInfo**, nuint, OrtStatusHandle> SessionGetMemoryInfoForOutputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const OrtEpDevice **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtEpDevice**, nuint, OrtStatusHandle> SessionGetEpDeviceForInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpDevice *, const OrtKeyValuePairs *, OrtSyncStream **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, OrtKeyValuePairs*, OrtSyncStream**, OrtStatusHandle> CreateSyncStreamForEpDevice;

        [NativeTypeName("void *(*)(OrtSyncStream *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSyncStream*, void*> SyncStream_GetHandle;

        [NativeTypeName("void (*)(OrtSyncStream *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSyncStream*, void> ReleaseSyncStream;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const OrtValue *const *, OrtValue *const *, OrtSyncStream *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtValue**, OrtValue**, OrtSyncStream*, nuint, OrtStatusHandle> CopyTensors;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, OrtModelMetadata **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtModelMetadata**, OrtStatusHandle> Graph_GetModelMetadata;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpDevice *const *, size_t, const char *, OrtCompiledModelCompatibility *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice**, nuint, sbyte*, OrtCompiledModelCompatibility*, OrtStatusHandle> GetModelCompatibilityForEpDevices;

        [NativeTypeName("OrtStatusPtr (*)(const wchar_t *, int64_t, size_t, OrtExternalInitializerInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<ushort*, long, nuint, OrtExternalInitializerInfo**, OrtStatusHandle> CreateExternalInitializerInfo;

        [NativeTypeName("_Bool (*)(const OrtTensorTypeAndShapeInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, byte> TensorTypeAndShape_HasShape;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, OrtKeyValuePairs **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, OrtKeyValuePairs**, OrtStatusHandle> KernelInfo_GetConfigEntries;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, nuint*, OrtStatusHandle> KernelInfo_GetOperatorDomain;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, nuint*, OrtStatusHandle> KernelInfo_GetOperatorType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, int*, OrtStatusHandle> KernelInfo_GetOperatorSinceVersion;

        [NativeTypeName("const OrtInteropApi *(*)(void) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtInteropApi*> GetInteropApi;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const OrtEpDevice **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtEpDevice**, nuint, OrtStatusHandle> SessionGetEpDeviceForOutputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, nuint*, OrtStatusHandle> GetNumHardwareDevices;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const OrtHardwareDevice **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtHardwareDevice**, nuint, OrtStatusHandle> GetHardwareDevices;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const char *, const OrtHardwareDevice *, OrtDeviceEpIncompatibilityDetails **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, sbyte*, OrtHardwareDevice*, OrtDeviceEpIncompatibilityDetails**, OrtStatusHandle> GetHardwareDeviceEpIncompatibilityDetails;

        [NativeTypeName("OrtStatusPtr (*)(const OrtDeviceEpIncompatibilityDetails *, uint32_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDeviceEpIncompatibilityDetails*, uint*, OrtStatusHandle> DeviceEpIncompatibilityDetails_GetReasonsBitmask;

        [NativeTypeName("OrtStatusPtr (*)(const OrtDeviceEpIncompatibilityDetails *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDeviceEpIncompatibilityDetails*, sbyte**, OrtStatusHandle> DeviceEpIncompatibilityDetails_GetNotes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtDeviceEpIncompatibilityDetails *, int32_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDeviceEpIncompatibilityDetails*, int*, OrtStatusHandle> DeviceEpIncompatibilityDetails_GetErrorCode;

        [NativeTypeName("void (*)(OrtDeviceEpIncompatibilityDetails *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDeviceEpIncompatibilityDetails*, void> ReleaseDeviceEpIncompatibilityDetails;

        [NativeTypeName("OrtStatusPtr (*)(const wchar_t *, const char *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<ushort*, sbyte*, OrtAllocator*, sbyte**, OrtStatusHandle> GetCompatibilityInfoFromModel;

        [NativeTypeName("OrtStatusPtr (*)(const void *, size_t, const char *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<void*, nuint, sbyte*, OrtAllocator*, sbyte**, OrtStatusHandle> GetCompatibilityInfoFromModelBytes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnvCreationOptions *, OrtEnv **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnvCreationOptions*, OrtEnv**, OrtStatusHandle> CreateEnvWithOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const OrtEpAssignedSubgraph *const **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtEpAssignedSubgraph***, nuint*, OrtStatusHandle> Session_GetEpGraphAssignmentInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpAssignedSubgraph *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpAssignedSubgraph*, sbyte**, OrtStatusHandle> EpAssignedSubgraph_GetEpName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpAssignedSubgraph *, const OrtEpAssignedNode *const **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpAssignedSubgraph*, OrtEpAssignedNode***, nuint*, OrtStatusHandle> EpAssignedSubgraph_GetNodes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpAssignedNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpAssignedNode*, sbyte**, OrtStatusHandle> EpAssignedNode_GetName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpAssignedNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpAssignedNode*, sbyte**, OrtStatusHandle> EpAssignedNode_GetDomain;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpAssignedNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpAssignedNode*, sbyte**, OrtStatusHandle> EpAssignedNode_GetOperatorType;

        [NativeTypeName("void (*)(OrtRunOptions *, OrtSyncStream *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, OrtSyncStream*, void> RunOptionsSetSyncStream;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, ONNXTensorElementDataType *, const int64_t **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, ONNXTensorElementDataType*, long**, nuint*, OrtStatusHandle> GetTensorElementTypeAndShapeDataReference;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, ushort*, OrtStatusHandle> RunOptionsEnableProfiling;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, OrtStatusHandle> RunOptionsDisableProfiling;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, OrtAllocator *, char ***, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, OrtAllocator*, sbyte***, nuint*, OrtStatusHandle> KernelInfoGetAttributeArray_string;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const OrtThreadPoolCallbacksConfig *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtThreadPoolCallbacksConfig*, OrtStatusHandle> SetPerSessionThreadPoolCallbacks;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSessionOptions *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, int*, OrtStatusHandle> GetMemPatternEnabled;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSessionOptions *, ExecutionMode *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, ExecutionMode*, OrtStatusHandle> GetSessionExecutionMode;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, int, OrtStatusHandle> SessionReleaseCapturedGraph;

        [NativeTypeName("OrtExperimentalFnPtr (*)(const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, delegate* unmanaged[Stdcall]<void>> GetExperimentalFunction;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, OrtSyncStream **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, OrtSyncStream**, OrtStatusHandle> KernelContext_GetSyncStream;
    }

    public enum OrtCustomOpInputOutputCharacteristic
    {
        INPUT_OUTPUT_REQUIRED = 0,
        INPUT_OUTPUT_OPTIONAL,
        INPUT_OUTPUT_VARIADIC,
    }

    public unsafe partial struct OrtCustomOp
    {
        [NativeTypeName("uint32_t")]
        public uint version;

        [NativeTypeName("void *(*)(const struct OrtCustomOp *, const OrtApi *, const OrtKernelInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, OrtApi*, OrtKernelInfo*, void*> CreateKernel;

        [NativeTypeName("const char *(*)(const struct OrtCustomOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, sbyte*> GetName;

        [NativeTypeName("const char *(*)(const struct OrtCustomOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, sbyte*> GetExecutionProviderType;

        [NativeTypeName("ONNXTensorElementDataType (*)(const struct OrtCustomOp *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, nuint, ONNXTensorElementDataType> GetInputType;

        [NativeTypeName("size_t (*)(const struct OrtCustomOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, nuint> GetInputTypeCount;

        [NativeTypeName("ONNXTensorElementDataType (*)(const struct OrtCustomOp *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, nuint, ONNXTensorElementDataType> GetOutputType;

        [NativeTypeName("size_t (*)(const struct OrtCustomOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, nuint> GetOutputTypeCount;

        [NativeTypeName("void (*)(void *, OrtKernelContext *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<void*, OrtKernelContext*, void> KernelCompute;

        [NativeTypeName("void (*)(void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<void*, void> KernelDestroy;

        [NativeTypeName("OrtCustomOpInputOutputCharacteristic (*)(const struct OrtCustomOp *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, nuint, OrtCustomOpInputOutputCharacteristic> GetInputCharacteristic;

        [NativeTypeName("OrtCustomOpInputOutputCharacteristic (*)(const struct OrtCustomOp *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, nuint, OrtCustomOpInputOutputCharacteristic> GetOutputCharacteristic;

        [NativeTypeName("OrtMemType (*)(const struct OrtCustomOp *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, nuint, OrtMemType> GetInputMemoryType;

        [NativeTypeName("int (*)(const struct OrtCustomOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, int> GetVariadicInputMinArity;

        [NativeTypeName("int (*)(const struct OrtCustomOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, int> GetVariadicInputHomogeneity;

        [NativeTypeName("int (*)(const struct OrtCustomOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, int> GetVariadicOutputMinArity;

        [NativeTypeName("int (*)(const struct OrtCustomOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, int> GetVariadicOutputHomogeneity;

        [NativeTypeName("OrtStatusPtr (*)(const struct OrtCustomOp *, const OrtApi *, const OrtKernelInfo *, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, OrtApi*, OrtKernelInfo*, void**, OrtStatusHandle> CreateKernelV2;

        [NativeTypeName("OrtStatusPtr (*)(void *, OrtKernelContext *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<void*, OrtKernelContext*, OrtStatusHandle> KernelComputeV2;

        [NativeTypeName("OrtStatusPtr (*)(const struct OrtCustomOp *, OrtShapeInferContext *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, OrtShapeInferContext*, OrtStatusHandle> InferOutputShapeFn;

        [NativeTypeName("int (*)(const struct OrtCustomOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, int> GetStartVersion;

        [NativeTypeName("int (*)(const struct OrtCustomOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, int> GetEndVersion;

        [NativeTypeName("size_t (*)(int **, int **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<int**, int**, nuint> GetMayInplace;

        [NativeTypeName("void (*)(int *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<int*, int*, void> ReleaseMayInplace;

        [NativeTypeName("size_t (*)(int **, int **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<int**, int**, nuint> GetAliasMap;

        [NativeTypeName("void (*)(int *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<int*, int*, void> ReleaseAliasMap;
    }

    public unsafe partial struct OrtModelEditorApi
    {
        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, OrtTypeInfo**, OrtStatusHandle> CreateTensorTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, OrtTypeInfo**, OrtStatusHandle> CreateSparseTensorTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(ONNXTensorElementDataType, const OrtTypeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<ONNXTensorElementDataType, OrtTypeInfo*, OrtTypeInfo**, OrtStatusHandle> CreateMapTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, OrtTypeInfo**, OrtStatusHandle> CreateSequenceTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, OrtTypeInfo**, OrtStatusHandle> CreateOptionalTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const char *, const OrtTypeInfo *, OrtValueInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, OrtTypeInfo*, OrtValueInfo**, OrtStatusHandle> CreateValueInfo;

        [NativeTypeName("OrtStatusPtr (*)(const char *, const char *, const char *, const char *const *, size_t, const char *const *, size_t, OrtOpAttr **, size_t, OrtNode **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, sbyte*, sbyte*, sbyte**, nuint, sbyte**, nuint, OrtOpAttr**, nuint, OrtNode**, OrtStatusHandle> CreateNode;

        [NativeTypeName("OrtStatusPtr (*)(OrtGraph **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph**, OrtStatusHandle> CreateGraph;

        [NativeTypeName("OrtStatusPtr (*)(OrtGraph *, OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtValueInfo**, nuint, OrtStatusHandle> SetGraphInputs;

        [NativeTypeName("OrtStatusPtr (*)(OrtGraph *, OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtValueInfo**, nuint, OrtStatusHandle> SetGraphOutputs;

        [NativeTypeName("OrtStatusPtr (*)(OrtGraph *, const char *, OrtValue *, _Bool) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, sbyte*, OrtValue*, byte, OrtStatusHandle> AddInitializerToGraph;

        [NativeTypeName("OrtStatusPtr (*)(OrtGraph *, OrtNode *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtNode*, OrtStatusHandle> AddNodeToGraph;

        [NativeTypeName("OrtStatusPtr (*)(const char *const *, const int *, size_t, OrtModel **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte**, int*, nuint, OrtModel**, OrtStatusHandle> CreateModel;

        [NativeTypeName("OrtStatusPtr (*)(OrtModel *, OrtGraph *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModel*, OrtGraph*, OrtStatusHandle> AddGraphToModel;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const OrtModel *, const OrtSessionOptions *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtModel*, OrtSessionOptions*, OrtSession**, OrtStatusHandle> CreateSessionFromModel;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const wchar_t *, const OrtSessionOptions *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, ushort*, OrtSessionOptions*, OrtSession**, OrtStatusHandle> CreateModelEditorSession;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const void *, size_t, const OrtSessionOptions *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, void*, nuint, OrtSessionOptions*, OrtSession**, OrtStatusHandle> CreateModelEditorSessionFromArray;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const char *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, sbyte*, int*, OrtStatusHandle> SessionGetOpsetForDomain;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, OrtModel *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtModel*, OrtStatusHandle> ApplyModelToModelEditorSession;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, const OrtSessionOptions *, OrtPrepackedWeightsContainer *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtSessionOptions*, OrtPrepackedWeightsContainer*, OrtStatusHandle> FinalizeModelEditorSession;
    }

    public enum OrtCompileApiFlags
    {
        OrtCompileApiFlags_NONE = 0,
        OrtCompileApiFlags_ERROR_IF_NO_NODES_COMPILED = 1 << 0,
        OrtCompileApiFlags_ERROR_IF_OUTPUT_FILE_EXISTS = 1 << 1,
    }

    public unsafe partial struct OrtCompileApi
    {
        [NativeTypeName("void (*)(OrtModelCompilationOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, void> ReleaseModelCompilationOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const OrtSessionOptions *, OrtModelCompilationOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtSessionOptions*, OrtModelCompilationOptions**, OrtStatusHandle> CreateModelCompilationOptionsFromSessionOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, ushort*, OrtStatusHandle> ModelCompilationOptions_SetInputModelPath;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, const void *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, void*, nuint, OrtStatusHandle> ModelCompilationOptions_SetInputModelFromBuffer;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, ushort*, OrtStatusHandle> ModelCompilationOptions_SetOutputModelPath;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, const wchar_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, ushort*, nuint, OrtStatusHandle> ModelCompilationOptions_SetOutputModelExternalInitializersFile;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, OrtAllocator *, void **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, OrtAllocator*, void**, nuint*, OrtStatusHandle> ModelCompilationOptions_SetOutputModelBuffer;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, _Bool) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, byte, OrtStatusHandle> ModelCompilationOptions_SetEpContextEmbedMode;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const OrtModelCompilationOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtModelCompilationOptions*, OrtStatusHandle> CompileModel;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, uint32_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, uint, OrtStatusHandle> ModelCompilationOptions_SetFlags;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, const wchar_t *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, ushort*, ushort*, OrtStatusHandle> ModelCompilationOptions_SetEpContextBinaryInformation;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, GraphOptimizationLevel) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, GraphOptimizationLevel, OrtStatusHandle> ModelCompilationOptions_SetGraphOptimizationLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, OrtWriteBufferFunc, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, delegate* unmanaged[Stdcall]<void*, void*, nuint, void*>, void*, OrtStatusHandle> ModelCompilationOptions_SetOutputModelWriteFunc;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, OrtGetInitializerLocationFunc, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, delegate* unmanaged[Stdcall]<void*, sbyte*, OrtValue*, OrtExternalInitializerInfo*, OrtExternalInitializerInfo**, void*>, void*, OrtStatusHandle> ModelCompilationOptions_SetOutputModelGetInitializerLocationFunc;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, const OrtModel *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, OrtModel*, OrtStatusHandle> ModelCompilationOptions_SetInputModel;
    }

    public unsafe partial struct OrtInteropApi
    {
        [NativeTypeName("OrtStatusPtr (*)(const OrtEpDevice *, OrtExternalResourceImporter **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, OrtExternalResourceImporter**, OrtStatusHandle> CreateExternalResourceImporterForDevice;

        [NativeTypeName("void (*)(OrtExternalResourceImporter *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, void> ReleaseExternalResourceImporter;

        [NativeTypeName("OrtStatusPtr (*)(const OrtExternalResourceImporter *, OrtExternalMemoryHandleType, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalMemoryHandleType, bool*, OrtStatusHandle> CanImportMemory;

        [NativeTypeName("OrtStatusPtr (*)(OrtExternalResourceImporter *, const OrtExternalMemoryDescriptor *, OrtExternalMemoryHandle **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalMemoryDescriptor*, OrtExternalMemoryHandle**, OrtStatusHandle> ImportMemory;

        [NativeTypeName("void (*)(OrtExternalMemoryHandle *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalMemoryHandle*, void> ReleaseExternalMemoryHandle;

        [NativeTypeName("OrtStatusPtr (*)(OrtExternalResourceImporter *, const OrtExternalMemoryHandle *, const OrtExternalTensorDescriptor *, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalMemoryHandle*, OrtExternalTensorDescriptor*, OrtValue**, OrtStatusHandle> CreateTensorFromMemory;

        [NativeTypeName("OrtStatusPtr (*)(const OrtExternalResourceImporter *, OrtExternalSemaphoreType, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalSemaphoreType, bool*, OrtStatusHandle> CanImportSemaphore;

        [NativeTypeName("OrtStatusPtr (*)(OrtExternalResourceImporter *, const OrtExternalSemaphoreDescriptor *, OrtExternalSemaphoreHandle **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalSemaphoreDescriptor*, OrtExternalSemaphoreHandle**, OrtStatusHandle> ImportSemaphore;

        [NativeTypeName("void (*)(OrtExternalSemaphoreHandle *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalSemaphoreHandle*, void> ReleaseExternalSemaphoreHandle;

        [NativeTypeName("OrtStatusPtr (*)(OrtExternalResourceImporter *, OrtExternalSemaphoreHandle *, OrtSyncStream *, uint64_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalSemaphoreHandle*, OrtSyncStream*, ulong, OrtStatusHandle> WaitSemaphore;

        [NativeTypeName("OrtStatusPtr (*)(OrtExternalResourceImporter *, OrtExternalSemaphoreHandle *, OrtSyncStream *, uint64_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalSemaphoreHandle*, OrtSyncStream*, ulong, OrtStatusHandle> SignalSemaphore;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpDevice *, const OrtGraphicsInteropConfig *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, OrtGraphicsInteropConfig*, OrtStatusHandle> InitGraphicsInteropForEpDevice;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpDevice *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, OrtStatusHandle> DeinitGraphicsInteropForEpDevice;
    }

    internal static unsafe partial class NativeExports
    {
        [DllImport("onnxruntime", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: NativeTypeName("const OrtApiBase *")]
        public static extern OrtApiBase* OrtGetApiBase();

        [DllImport("onnxruntime", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: NativeTypeName("OrtStatusPtr")]
        public static extern OrtStatusHandle OrtSessionOptionsAppendExecutionProvider_CUDA(OrtSessionOptions* options, int device_id);

        [DllImport("onnxruntime", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: NativeTypeName("OrtStatusPtr")]
        public static extern OrtStatusHandle OrtSessionOptionsAppendExecutionProvider_ROCM(OrtSessionOptions* options, int device_id);

        [DllImport("onnxruntime", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: NativeTypeName("OrtStatusPtr")]
        public static extern OrtStatusHandle OrtSessionOptionsAppendExecutionProvider_MIGraphX(OrtSessionOptions* options, int device_id);

        [DllImport("onnxruntime", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: NativeTypeName("OrtStatusPtr")]
        public static extern OrtStatusHandle OrtSessionOptionsAppendExecutionProvider_Dnnl(OrtSessionOptions* options, int use_arena);

        [DllImport("onnxruntime", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: NativeTypeName("OrtStatusPtr")]
        public static extern OrtStatusHandle OrtSessionOptionsAppendExecutionProvider_Tensorrt(OrtSessionOptions* options, int device_id);
    }
}
