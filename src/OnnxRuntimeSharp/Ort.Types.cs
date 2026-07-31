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
    public readonly struct OrtStatus;
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
        public delegate* unmanaged[Stdcall]<OrtAllocator*, OrtKeyValuePairs**, OrtStatus*> GetStats;

        [NativeTypeName("void *(*)(struct OrtAllocator *, size_t, OrtSyncStream *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, nuint, OrtSyncStream*, void*> AllocOnStream;

        [NativeTypeName("OrtStatusPtr (*)(struct OrtAllocator *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, OrtStatus*> Shrink;
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
        public delegate* unmanaged[Stdcall]<OrtErrorCode, sbyte*, OrtStatus*> CreateStatus;

        [NativeTypeName("OrtErrorCode (*)(const OrtStatus *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtStatus*, OrtErrorCode> GetErrorCode;

        [NativeTypeName("const char *(*)(const OrtStatus *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtStatus*, sbyte*> GetErrorMessage;

        [NativeTypeName("OrtStatusPtr (*)(OrtLoggingLevel, const char *, OrtEnv **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtLoggingLevel, sbyte*, OrtEnv**, OrtStatus*> CreateEnv;

        [NativeTypeName("OrtStatusPtr (*)(OrtLoggingFunction, void *, OrtLoggingLevel, const char *, OrtEnv **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<delegate* unmanaged[Stdcall]<void*, OrtLoggingLevel, sbyte*, sbyte*, sbyte*, sbyte*, void>, void*, OrtLoggingLevel, sbyte*, OrtEnv**, OrtStatus*> CreateEnvWithCustomLogger;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtStatus*> EnableTelemetryEvents;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtStatus*> DisableTelemetryEvents;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const wchar_t *, const OrtSessionOptions *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, ushort*, OrtSessionOptions*, OrtSession**, OrtStatus*> CreateSession;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const void *, size_t, const OrtSessionOptions *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, void*, nuint, OrtSessionOptions*, OrtSession**, OrtStatus*> CreateSessionFromArray;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, const OrtRunOptions *, const char *const *, const OrtValue *const *, size_t, const char *const *, size_t, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtRunOptions*, sbyte**, OrtValue**, nuint, sbyte**, nuint, OrtValue**, OrtStatus*> Run;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions**, OrtStatus*> CreateSessionOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, ushort*, OrtStatus*> SetOptimizedModelFilePath;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSessionOptions *, OrtSessionOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtSessionOptions**, OrtStatus*> CloneSessionOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, ExecutionMode) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, ExecutionMode, OrtStatus*> SetSessionExecutionMode;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, ushort*, OrtStatus*> EnableProfiling;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatus*> DisableProfiling;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatus*> EnableMemPattern;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatus*> DisableMemPattern;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatus*> EnableCpuMemArena;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatus*> DisableCpuMemArena;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, OrtStatus*> SetSessionLogId;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, int, OrtStatus*> SetSessionLogVerbosityLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, int, OrtStatus*> SetSessionLogSeverityLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, GraphOptimizationLevel) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, GraphOptimizationLevel, OrtStatus*> SetSessionGraphOptimizationLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, int, OrtStatus*> SetIntraOpNumThreads;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, int, OrtStatus*> SetInterOpNumThreads;

        [NativeTypeName("OrtStatusPtr (*)(const char *, OrtCustomOpDomain **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, OrtCustomOpDomain**, OrtStatus*> CreateCustomOpDomain;

        [NativeTypeName("OrtStatusPtr (*)(OrtCustomOpDomain *, const OrtCustomOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOpDomain*, OrtCustomOp*, OrtStatus*> CustomOpDomain_Add;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, OrtCustomOpDomain *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtCustomOpDomain*, OrtStatus*> AddCustomOpDomain;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, void**, OrtStatus*> RegisterCustomOpsLibrary;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint*, OrtStatus*> SessionGetInputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint*, OrtStatus*> SessionGetOutputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint*, OrtStatus*> SessionGetOverridableInitializerCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint, OrtTypeInfo**, OrtStatus*> SessionGetInputTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint, OrtTypeInfo**, OrtStatus*> SessionGetOutputTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint, OrtTypeInfo**, OrtStatus*> SessionGetOverridableInitializerTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint, OrtAllocator*, sbyte**, OrtStatus*> SessionGetInputName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint, OrtAllocator*, sbyte**, OrtStatus*> SessionGetOutputName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, size_t, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, nuint, OrtAllocator*, sbyte**, OrtStatus*> SessionGetOverridableInitializerName;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions**, OrtStatus*> CreateRunOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, int, OrtStatus*> RunOptionsSetRunLogVerbosityLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, int, OrtStatus*> RunOptionsSetRunLogSeverityLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, sbyte*, OrtStatus*> RunOptionsSetRunTag;

        [NativeTypeName("OrtStatusPtr (*)(const OrtRunOptions *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, int*, OrtStatus*> RunOptionsGetRunLogVerbosityLevel;

        [NativeTypeName("OrtStatusPtr (*)(const OrtRunOptions *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, int*, OrtStatus*> RunOptionsGetRunLogSeverityLevel;

        [NativeTypeName("OrtStatusPtr (*)(const OrtRunOptions *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, sbyte**, OrtStatus*> RunOptionsGetRunTag;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, OrtStatus*> RunOptionsSetTerminate;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, OrtStatus*> RunOptionsUnsetTerminate;

        [NativeTypeName("OrtStatusPtr (*)(OrtAllocator *, const int64_t *, size_t, ONNXTensorElementDataType, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, long*, nuint, ONNXTensorElementDataType, OrtValue**, OrtStatus*> CreateTensorAsOrtValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, void *, size_t, const int64_t *, size_t, ONNXTensorElementDataType, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, void*, nuint, long*, nuint, ONNXTensorElementDataType, OrtValue**, OrtStatus*> CreateTensorWithDataAsOrtValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, int*, OrtStatus*> IsTensor;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, void**, OrtStatus*> GetTensorMutableData;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, sbyte**, nuint, OrtStatus*> FillStringTensor;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, nuint*, OrtStatus*> GetStringTensorDataLength;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, void *, size_t, size_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, void*, nuint, nuint*, nuint, OrtStatus*> GetStringTensorContent;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, const OrtTensorTypeAndShapeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, OrtTensorTypeAndShapeInfo**, OrtStatus*> CastTypeInfoToTensorInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, enum ONNXType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, ONNXType*, OrtStatus*> GetOnnxTypeFromTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorTypeAndShapeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo**, OrtStatus*> CreateTensorTypeAndShapeInfo;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorTypeAndShapeInfo *, enum ONNXTensorElementDataType) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, ONNXTensorElementDataType, OrtStatus*> SetTensorElementType;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorTypeAndShapeInfo *, const int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, long*, nuint, OrtStatus*> SetDimensions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, enum ONNXTensorElementDataType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, ONNXTensorElementDataType*, OrtStatus*> GetTensorElementType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, nuint*, OrtStatus*> GetDimensionsCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, long*, nuint, OrtStatus*> GetDimensions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, const char **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, sbyte*, nuint, OrtStatus*> GetSymbolicDimensions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, nuint*, OrtStatus*> GetTensorShapeElementCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, OrtTensorTypeAndShapeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtTensorTypeAndShapeInfo**, OrtStatus*> GetTensorTypeAndShape;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtTypeInfo**, OrtStatus*> GetTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, enum ONNXType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, ONNXType*, OrtStatus*> GetValueType;

        [NativeTypeName("OrtStatusPtr (*)(const char *, enum OrtAllocatorType, int, enum OrtMemType, OrtMemoryInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, OrtAllocatorType, int, OrtMemType, OrtMemoryInfo**, OrtStatus*> CreateMemoryInfo;

        [NativeTypeName("OrtStatusPtr (*)(enum OrtAllocatorType, enum OrtMemType, OrtMemoryInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocatorType, OrtMemType, OrtMemoryInfo**, OrtStatus*> CreateCpuMemoryInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, const OrtMemoryInfo *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, OrtMemoryInfo*, int*, OrtStatus*> CompareMemoryInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, sbyte**, OrtStatus*> MemoryInfoGetName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, int*, OrtStatus*> MemoryInfoGetId;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, OrtMemType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, OrtMemType*, OrtStatus*> MemoryInfoGetMemType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, OrtAllocatorType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, OrtAllocatorType*, OrtStatus*> MemoryInfoGetType;

        [NativeTypeName("OrtStatusPtr (*)(OrtAllocator *, size_t, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, nuint, void**, OrtStatus*> AllocatorAlloc;

        [NativeTypeName("OrtStatusPtr (*)(OrtAllocator *, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, void*, OrtStatus*> AllocatorFree;

        [NativeTypeName("OrtStatusPtr (*)(const OrtAllocator *, const struct OrtMemoryInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, OrtMemoryInfo**, OrtStatus*> AllocatorGetInfo;

        [NativeTypeName("OrtStatusPtr (*)(OrtAllocator **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator**, OrtStatus*> GetAllocatorWithDefaultOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *, int64_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, long, OrtStatus*> AddFreeDimensionOverride;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, int, OrtAllocator *, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, int, OrtAllocator*, OrtValue**, OrtStatus*> GetValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, nuint*, OrtStatus*> GetValueCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *const *, size_t, enum ONNXType, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue**, nuint, ONNXType, OrtValue**, OrtStatus*> CreateValue;

        [NativeTypeName("OrtStatusPtr (*)(const char *, const char *, const void *, size_t, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, sbyte*, void*, nuint, OrtValue**, OrtStatus*> CreateOpaqueValue;

        [NativeTypeName("OrtStatusPtr (*)(const char *, const char *, const OrtValue *, void *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, sbyte*, OrtValue*, void*, nuint, OrtStatus*> GetOpaqueValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, float *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, float*, OrtStatus*> KernelInfoGetAttribute_float;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, int64_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, long*, OrtStatus*> KernelInfoGetAttribute_int64;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, sbyte*, nuint*, OrtStatus*> KernelInfoGetAttribute_string;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, nuint*, OrtStatus*> KernelContext_GetInputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, nuint*, OrtStatus*> KernelContext_GetOutputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, size_t, const OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, nuint, OrtValue**, OrtStatus*> KernelContext_GetInput;

        [NativeTypeName("OrtStatusPtr (*)(OrtKernelContext *, size_t, const int64_t *, size_t, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, nuint, long*, nuint, OrtValue**, OrtStatus*> KernelContext_GetOutput;

        [NativeTypeName("void (*)(OrtEnv *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, void> ReleaseEnv;

        [NativeTypeName("void (*)(OrtStatus *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtStatus*, void> ReleaseStatus;

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
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, sbyte**, nuint*, OrtStatus*> GetDenotationFromTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, const OrtMapTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, OrtMapTypeInfo**, OrtStatus*> CastTypeInfoToMapTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, const OrtSequenceTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, OrtSequenceTypeInfo**, OrtStatus*> CastTypeInfoToSequenceTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMapTypeInfo *, enum ONNXTensorElementDataType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMapTypeInfo*, ONNXTensorElementDataType*, OrtStatus*> GetMapKeyType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMapTypeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMapTypeInfo*, OrtTypeInfo**, OrtStatus*> GetMapValueType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSequenceTypeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSequenceTypeInfo*, OrtTypeInfo**, OrtStatus*> GetSequenceElementType;

        [NativeTypeName("void (*)(OrtMapTypeInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMapTypeInfo*, void> ReleaseMapTypeInfo;

        [NativeTypeName("void (*)(OrtSequenceTypeInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSequenceTypeInfo*, void> ReleaseSequenceTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtAllocator*, sbyte**, OrtStatus*> SessionEndProfiling;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, OrtModelMetadata **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtModelMetadata**, OrtStatus*> SessionGetModelMetadata;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte**, OrtStatus*> ModelMetadataGetProducerName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte**, OrtStatus*> ModelMetadataGetGraphName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte**, OrtStatus*> ModelMetadataGetDomain;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte**, OrtStatus*> ModelMetadataGetDescription;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, const char *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte*, sbyte**, OrtStatus*> ModelMetadataLookupCustomMetadataMap;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, int64_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, long*, OrtStatus*> ModelMetadataGetVersion;

        [NativeTypeName("void (*)(OrtModelMetadata *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, void> ReleaseModelMetadata;

        [NativeTypeName("OrtStatusPtr (*)(OrtLoggingLevel, const char *, const OrtThreadingOptions *, OrtEnv **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtLoggingLevel, sbyte*, OrtThreadingOptions*, OrtEnv**, OrtStatus*> CreateEnvWithGlobalThreadPools;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatus*> DisablePerSessionThreads;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions**, OrtStatus*> CreateThreadingOptions;

        [NativeTypeName("void (*)(OrtThreadingOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, void> ReleaseThreadingOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, char ***, int64_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte***, long*, OrtStatus*> ModelMetadataGetCustomMetadataMapKeys;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *, int64_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, long, OrtStatus*> AddFreeDimensionOverrideByName;

        [NativeTypeName("OrtStatusPtr (*)(char ***, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte***, int*, OrtStatus*> GetAvailableProviders;

        [NativeTypeName("OrtStatusPtr (*)(char **, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte**, int, OrtStatus*> ReleaseAvailableProviders;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, size_t, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, nuint, nuint*, OrtStatus*> GetStringTensorElementLength;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, size_t, size_t, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, nuint, nuint, void*, OrtStatus*> GetStringTensorElement;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const char *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, sbyte*, nuint, OrtStatus*> FillStringTensorElement;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, sbyte*, OrtStatus*> AddSessionConfigEntry;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const OrtMemoryInfo *, OrtAllocator **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtMemoryInfo*, OrtAllocator**, OrtStatus*> CreateAllocator;

        [NativeTypeName("void (*)(OrtAllocator *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, void> ReleaseAllocator;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, const OrtRunOptions *, const OrtIoBinding *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtRunOptions*, OrtIoBinding*, OrtStatus*> RunWithBinding;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, OrtIoBinding **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtIoBinding**, OrtStatus*> CreateIoBinding;

        [NativeTypeName("void (*)(OrtIoBinding *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, void> ReleaseIoBinding;

        [NativeTypeName("OrtStatusPtr (*)(OrtIoBinding *, const char *, const OrtValue *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, sbyte*, OrtValue*, OrtStatus*> BindInput;

        [NativeTypeName("OrtStatusPtr (*)(OrtIoBinding *, const char *, const OrtValue *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, sbyte*, OrtValue*, OrtStatus*> BindOutput;

        [NativeTypeName("OrtStatusPtr (*)(OrtIoBinding *, const char *, const OrtMemoryInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, sbyte*, OrtMemoryInfo*, OrtStatus*> BindOutputToDevice;

        [NativeTypeName("OrtStatusPtr (*)(const OrtIoBinding *, OrtAllocator *, char **, size_t **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, OrtAllocator*, sbyte**, nuint**, nuint*, OrtStatus*> GetBoundOutputNames;

        [NativeTypeName("OrtStatusPtr (*)(const OrtIoBinding *, OrtAllocator *, OrtValue ***, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, OrtAllocator*, OrtValue***, nuint*, OrtStatus*> GetBoundOutputValues;

        [NativeTypeName("void (*)(OrtIoBinding *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, void> ClearBoundInputs;

        [NativeTypeName("void (*)(OrtIoBinding *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, void> ClearBoundOutputs;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const int64_t *, size_t, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, long*, nuint, void**, OrtStatus*> TensorAt;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const OrtMemoryInfo *, const OrtArenaCfg *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtMemoryInfo*, OrtArenaCfg*, OrtStatus*> CreateAndRegisterAllocator;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, OrtLanguageProjection) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtLanguageProjection, OrtStatus*> SetLanguageProjection;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, uint64_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, ulong*, OrtStatus*> SessionGetProfilingStartTimeNs;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, int, OrtStatus*> SetGlobalIntraOpNumThreads;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, int, OrtStatus*> SetGlobalInterOpNumThreads;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, int, OrtStatus*> SetGlobalSpinControl;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *, const OrtValue *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, OrtValue*, OrtStatus*> AddInitializer;

        [NativeTypeName("OrtStatusPtr (*)(OrtLoggingFunction, void *, OrtLoggingLevel, const char *, const struct OrtThreadingOptions *, OrtEnv **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<delegate* unmanaged[Stdcall]<void*, OrtLoggingLevel, sbyte*, sbyte*, sbyte*, sbyte*, void>, void*, OrtLoggingLevel, sbyte*, OrtThreadingOptions*, OrtEnv**, OrtStatus*> CreateEnvWithCustomLoggerAndGlobalThreadPools;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtCUDAProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtCUDAProviderOptions*, OrtStatus*> SessionOptionsAppendExecutionProvider_CUDA;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtROCMProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtROCMProviderOptions*, OrtStatus*> SessionOptionsAppendExecutionProvider_ROCM;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtOpenVINOProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtOpenVINOProviderOptions*, OrtStatus*> SessionOptionsAppendExecutionProvider_OpenVINO;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, OrtStatus*> SetGlobalDenormalAsZero;

        [NativeTypeName("OrtStatusPtr (*)(size_t, int, int, int, OrtArenaCfg **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<nuint, int, int, int, OrtArenaCfg**, OrtStatus*> CreateArenaCfg;

        [NativeTypeName("void (*)(OrtArenaCfg *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtArenaCfg*, void> ReleaseArenaCfg;

        [NativeTypeName("OrtStatusPtr (*)(const OrtModelMetadata *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelMetadata*, OrtAllocator*, sbyte**, OrtStatus*> ModelMetadataGetGraphDescription;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtTensorRTProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtTensorRTProviderOptions*, OrtStatus*> SessionOptionsAppendExecutionProvider_TensorRT;

        [NativeTypeName("OrtStatusPtr (*)(int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<int, OrtStatus*> SetCurrentGpuDeviceId;

        [NativeTypeName("OrtStatusPtr (*)(int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<int*, OrtStatus*> GetCurrentGpuDeviceId;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, float *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, float*, nuint*, OrtStatus*> KernelInfoGetAttributeArray_float;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, int64_t *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, long*, nuint*, OrtStatus*> KernelInfoGetAttributeArray_int64;

        [NativeTypeName("OrtStatusPtr (*)(const char *const *, const size_t *, size_t, OrtArenaCfg **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte**, nuint*, nuint, OrtArenaCfg**, OrtStatus*> CreateArenaCfgV2;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *, const char *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, sbyte*, sbyte*, OrtStatus*> AddRunConfigEntry;

        [NativeTypeName("OrtStatusPtr (*)(OrtPrepackedWeightsContainer **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtPrepackedWeightsContainer**, OrtStatus*> CreatePrepackedWeightsContainer;

        [NativeTypeName("void (*)(OrtPrepackedWeightsContainer *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtPrepackedWeightsContainer*, void> ReleasePrepackedWeightsContainer;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const wchar_t *, const OrtSessionOptions *, OrtPrepackedWeightsContainer *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, ushort*, OrtSessionOptions*, OrtPrepackedWeightsContainer*, OrtSession**, OrtStatus*> CreateSessionWithPrepackedWeightsContainer;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const void *, size_t, const OrtSessionOptions *, OrtPrepackedWeightsContainer *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, void*, nuint, OrtSessionOptions*, OrtPrepackedWeightsContainer*, OrtSession**, OrtStatus*> CreateSessionFromArrayWithPrepackedWeightsContainer;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtTensorRTProviderOptionsV2 *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtTensorRTProviderOptionsV2*, OrtStatus*> SessionOptionsAppendExecutionProvider_TensorRT_V2;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorRTProviderOptionsV2 **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorRTProviderOptionsV2**, OrtStatus*> CreateTensorRTProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorRTProviderOptionsV2 *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorRTProviderOptionsV2*, sbyte**, sbyte**, nuint, OrtStatus*> UpdateTensorRTProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorRTProviderOptionsV2 *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorRTProviderOptionsV2*, OrtAllocator*, sbyte**, OrtStatus*> GetTensorRTProviderOptionsAsString;

        [NativeTypeName("void (*)(OrtTensorRTProviderOptionsV2 *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorRTProviderOptionsV2*, void> ReleaseTensorRTProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtStatus*> EnableOrtCustomOps;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, OrtAllocator *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtAllocator*, OrtStatus*> RegisterAllocator;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const OrtMemoryInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtMemoryInfo*, OrtStatus*> UnregisterAllocator;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, int*, OrtStatus*> IsSparseTensor;

        [NativeTypeName("OrtStatusPtr (*)(OrtAllocator *, const int64_t *, size_t, ONNXTensorElementDataType, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, long*, nuint, ONNXTensorElementDataType, OrtValue**, OrtStatus*> CreateSparseTensorAsOrtValue;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const OrtMemoryInfo *, const int64_t *, size_t, const void *, const int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtMemoryInfo*, long*, nuint, void*, long*, nuint, OrtStatus*> FillSparseTensorCoo;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const OrtMemoryInfo *, const int64_t *, size_t, const void *, const int64_t *, size_t, const int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtMemoryInfo*, long*, nuint, void*, long*, nuint, long*, nuint, OrtStatus*> FillSparseTensorCsr;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const OrtMemoryInfo *, const int64_t *, size_t, const void *, const int64_t *, size_t, const int32_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtMemoryInfo*, long*, nuint, void*, long*, nuint, int*, OrtStatus*> FillSparseTensorBlockSparse;

        [NativeTypeName("OrtStatusPtr (*)(const OrtMemoryInfo *, void *, const int64_t *, size_t, const int64_t *, size_t, ONNXTensorElementDataType, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, void*, long*, nuint, long*, nuint, ONNXTensorElementDataType, OrtValue**, OrtStatus*> CreateSparseTensorWithValuesAsOrtValue;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, long*, nuint, OrtStatus*> UseCooIndices;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, int64_t *, size_t, int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, long*, nuint, long*, nuint, OrtStatus*> UseCsrIndices;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, const int64_t *, size_t, int32_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, long*, nuint, int*, OrtStatus*> UseBlockSparseIndices;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, enum OrtSparseFormat *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtSparseFormat*, OrtStatus*> GetSparseTensorFormat;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, OrtTensorTypeAndShapeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtTensorTypeAndShapeInfo**, OrtStatus*> GetSparseTensorValuesTypeAndShape;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, const void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, void**, OrtStatus*> GetSparseTensorValues;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, enum OrtSparseIndicesFormat, OrtTensorTypeAndShapeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtSparseIndicesFormat, OrtTensorTypeAndShapeInfo**, OrtStatus*> GetSparseTensorIndicesTypeShape;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, enum OrtSparseIndicesFormat, size_t *, const void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtSparseIndicesFormat, nuint*, void**, OrtStatus*> GetSparseTensorIndices;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, int*, OrtStatus*> HasValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, void**, OrtStatus*> KernelContext_GetGPUComputeStream;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, const OrtMemoryInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, OrtMemoryInfo**, OrtStatus*> GetTensorMemoryInfo;

        [NativeTypeName("OrtStatusPtr (*)(const char *, uint32_t, const void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, uint, void**, OrtStatus*> GetExecutionProviderApi;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, OrtCustomCreateThreadFn) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, delegate* unmanaged[Cdecl]<void*, delegate* unmanaged[Cdecl]<void*, void>, void*, OrtCustomHandleType*>, OrtStatus*> SessionOptionsSetCustomCreateThreadFn;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, void*, OrtStatus*> SessionOptionsSetCustomThreadCreationOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, OrtCustomJoinThreadFn) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, delegate* unmanaged[Cdecl]<OrtCustomHandleType*, void>, OrtStatus*> SessionOptionsSetCustomJoinThreadFn;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, OrtCustomCreateThreadFn) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, delegate* unmanaged[Cdecl]<void*, delegate* unmanaged[Cdecl]<void*, void>, void*, OrtCustomHandleType*>, OrtStatus*> SetGlobalCustomCreateThreadFn;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, void*, OrtStatus*> SetGlobalCustomThreadCreationOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, OrtCustomJoinThreadFn) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, delegate* unmanaged[Cdecl]<OrtCustomHandleType*, void>, OrtStatus*> SetGlobalCustomJoinThreadFn;

        [NativeTypeName("OrtStatusPtr (*)(OrtIoBinding *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, OrtStatus*> SynchronizeBoundInputs;

        [NativeTypeName("OrtStatusPtr (*)(OrtIoBinding *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtIoBinding*, OrtStatus*> SynchronizeBoundOutputs;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtCUDAProviderOptionsV2 *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtCUDAProviderOptionsV2*, OrtStatus*> SessionOptionsAppendExecutionProvider_CUDA_V2;

        [NativeTypeName("OrtStatusPtr (*)(OrtCUDAProviderOptionsV2 **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCUDAProviderOptionsV2**, OrtStatus*> CreateCUDAProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtCUDAProviderOptionsV2 *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCUDAProviderOptionsV2*, sbyte**, sbyte**, nuint, OrtStatus*> UpdateCUDAProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtCUDAProviderOptionsV2 *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCUDAProviderOptionsV2*, OrtAllocator*, sbyte**, OrtStatus*> GetCUDAProviderOptionsAsString;

        [NativeTypeName("void (*)(OrtCUDAProviderOptionsV2 *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCUDAProviderOptionsV2*, void> ReleaseCUDAProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtMIGraphXProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtMIGraphXProviderOptions*, OrtStatus*> SessionOptionsAppendExecutionProvider_MIGraphX;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *const *, const OrtValue *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte**, OrtValue**, nuint, OrtStatus*> AddExternalInitializers;

        [NativeTypeName("OrtStatusPtr (*)(const char *, const void *, int, OrtOpAttrType, OrtOpAttr **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, void*, int, OrtOpAttrType, OrtOpAttr**, OrtStatus*> CreateOpAttr;

        [NativeTypeName("void (*)(OrtOpAttr *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOpAttr*, void> ReleaseOpAttr;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, const char *, int, const char **, const ONNXTensorElementDataType *, int, const OrtOpAttr *const *, int, int, int, OrtOp **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, sbyte*, int, sbyte**, ONNXTensorElementDataType*, int, OrtOpAttr**, int, int, int, OrtOp**, OrtStatus*> CreateOp;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, const OrtOp *, const OrtValue *const *, int, OrtValue *const *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, OrtOp*, OrtValue**, int, OrtValue**, int, OrtStatus*> InvokeOp;

        [NativeTypeName("void (*)(OrtOp *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOp*, void> ReleaseOp;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, sbyte**, sbyte**, nuint, OrtStatus*> SessionOptionsAppendExecutionProvider;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, OrtKernelInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, OrtKernelInfo**, OrtStatus*> CopyKernelInfo;

        [NativeTypeName("void (*)(OrtKernelInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, void> ReleaseKernelInfo;

        [NativeTypeName("const OrtTrainingApi *(*)(uint32_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<uint, OrtTrainingApi*> GetTrainingApi;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtCANNProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtCANNProviderOptions*, OrtStatus*> SessionOptionsAppendExecutionProvider_CANN;

        [NativeTypeName("OrtStatusPtr (*)(OrtCANNProviderOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCANNProviderOptions**, OrtStatus*> CreateCANNProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtCANNProviderOptions *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCANNProviderOptions*, sbyte**, sbyte**, nuint, OrtStatus*> UpdateCANNProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtCANNProviderOptions *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCANNProviderOptions*, OrtAllocator*, sbyte**, OrtStatus*> GetCANNProviderOptionsAsString;

        [NativeTypeName("void (*)(OrtCANNProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCANNProviderOptions*, void> ReleaseCANNProviderOptions;

        [NativeTypeName("void (*)(const OrtMemoryInfo *, OrtMemoryInfoDeviceType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, OrtMemoryInfoDeviceType*, void> MemoryInfoGetDeviceType;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, OrtLoggingLevel) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtLoggingLevel, OrtStatus*> UpdateEnvWithCustomLogLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtThreadingOptions *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtThreadingOptions*, sbyte*, OrtStatus*> SetGlobalIntraOpThreadAffinity;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, ushort*, OrtStatus*> RegisterCustomOpsLibrary_V2;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, OrtStatus*> RegisterCustomOpsUsingFunction;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint*, OrtStatus*> KernelInfo_GetInputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint*, OrtStatus*> KernelInfo_GetOutputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint, sbyte*, nuint*, OrtStatus*> KernelInfo_GetInputName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint, sbyte*, nuint*, OrtStatus*> KernelInfo_GetOutputName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint, OrtTypeInfo**, OrtStatus*> KernelInfo_GetInputTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint, OrtTypeInfo**, OrtStatus*> KernelInfo_GetOutputTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, OrtAllocator *, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, OrtAllocator*, OrtValue**, OrtStatus*> KernelInfoGetAttribute_tensor;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSessionOptions *, const char *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, int*, OrtStatus*> HasSessionConfigEntry;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSessionOptions *, const char *, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte*, sbyte*, nuint*, OrtStatus*> GetSessionConfigEntry;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const OrtDnnlProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtDnnlProviderOptions*, OrtStatus*> SessionOptionsAppendExecutionProvider_Dnnl;

        [NativeTypeName("OrtStatusPtr (*)(OrtDnnlProviderOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDnnlProviderOptions**, OrtStatus*> CreateDnnlProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtDnnlProviderOptions *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDnnlProviderOptions*, sbyte**, sbyte**, nuint, OrtStatus*> UpdateDnnlProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtDnnlProviderOptions *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDnnlProviderOptions*, OrtAllocator*, sbyte**, OrtStatus*> GetDnnlProviderOptionsAsString;

        [NativeTypeName("void (*)(OrtDnnlProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDnnlProviderOptions*, void> ReleaseDnnlProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, nuint*, OrtStatus*> KernelInfo_GetNodeName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const OrtLogger **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, OrtLogger**, OrtStatus*> KernelInfo_GetLogger;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, const OrtLogger **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, OrtLogger**, OrtStatus*> KernelContext_GetLogger;

        [NativeTypeName("OrtStatusPtr (*)(const OrtLogger *, OrtLoggingLevel, const char *, const wchar_t *, int, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtLogger*, OrtLoggingLevel, sbyte*, ushort*, int, sbyte*, OrtStatus*> Logger_LogMessage;

        [NativeTypeName("OrtStatusPtr (*)(const OrtLogger *, OrtLoggingLevel *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtLogger*, OrtLoggingLevel*, OrtStatus*> Logger_GetLoggingSeverityLevel;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, size_t, int *, const OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, nuint, int*, OrtValue**, OrtStatus*> KernelInfoGetConstantInput_tensor;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, const OrtOptionalTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, OrtOptionalTypeInfo**, OrtStatus*> CastTypeInfoToOptionalTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtOptionalTypeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOptionalTypeInfo*, OrtTypeInfo**, OrtStatus*> GetOptionalContainedTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(OrtValue *, size_t, size_t, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, nuint, nuint, sbyte**, OrtStatus*> GetResizedStringTensorElementBuffer;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, const OrtMemoryInfo *, OrtAllocator **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, OrtMemoryInfo*, OrtAllocator**, OrtStatus*> KernelContext_GetAllocator;

        [NativeTypeName("const char *(*)(void) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*> GetBuildInfoString;

        [NativeTypeName("OrtStatusPtr (*)(OrtROCMProviderOptions **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtROCMProviderOptions**, OrtStatus*> CreateROCMProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtROCMProviderOptions *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtROCMProviderOptions*, sbyte**, sbyte**, nuint, OrtStatus*> UpdateROCMProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtROCMProviderOptions *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtROCMProviderOptions*, OrtAllocator*, sbyte**, OrtStatus*> GetROCMProviderOptionsAsString;

        [NativeTypeName("void (*)(OrtROCMProviderOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtROCMProviderOptions*, void> ReleaseROCMProviderOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const char *, const OrtMemoryInfo *, const OrtArenaCfg *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, sbyte*, OrtMemoryInfo*, OrtArenaCfg*, sbyte**, sbyte**, nuint, OrtStatus*> CreateAndRegisterAllocatorV2;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, const OrtRunOptions *, const char *const *, const OrtValue *const *, size_t, const char *const *, size_t, OrtValue **, RunAsyncCallbackFn, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtRunOptions*, sbyte**, OrtValue**, nuint, sbyte**, nuint, OrtValue**, delegate* unmanaged[Cdecl]<void*, OrtValue**, nuint, OrtStatus*, void>, void*, OrtStatus*> RunAsync;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorRTProviderOptionsV2 *, const char *, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorRTProviderOptionsV2*, sbyte*, void*, OrtStatus*> UpdateTensorRTProviderOptionsWithValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorRTProviderOptionsV2 *, const char *, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorRTProviderOptionsV2*, sbyte*, void**, OrtStatus*> GetTensorRTProviderOptionsByName;

        [NativeTypeName("OrtStatusPtr (*)(OrtCUDAProviderOptionsV2 *, const char *, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCUDAProviderOptionsV2*, sbyte*, void*, OrtStatus*> UpdateCUDAProviderOptionsWithValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtCUDAProviderOptionsV2 *, const char *, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCUDAProviderOptionsV2*, sbyte*, void**, OrtStatus*> GetCUDAProviderOptionsByName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, int, int, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, int, int, void**, OrtStatus*> KernelContext_GetResource;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, OrtLoggingFunction, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, delegate* unmanaged[Stdcall]<void*, OrtLoggingLevel, sbyte*, sbyte*, sbyte*, sbyte*, void>, void*, OrtStatus*> SetUserLoggingFunction;

        [NativeTypeName("OrtStatusPtr (*)(const OrtShapeInferContext *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtShapeInferContext*, nuint*, OrtStatus*> ShapeInferContext_GetInputCount;

        [NativeTypeName("OrtStatusPtr (*)(const OrtShapeInferContext *, size_t, OrtTensorTypeAndShapeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtShapeInferContext*, nuint, OrtTensorTypeAndShapeInfo**, OrtStatus*> ShapeInferContext_GetInputTypeShape;

        [NativeTypeName("OrtStatusPtr (*)(const OrtShapeInferContext *, const char *, const OrtOpAttr **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtShapeInferContext*, sbyte*, OrtOpAttr**, OrtStatus*> ShapeInferContext_GetAttribute;

        [NativeTypeName("OrtStatusPtr (*)(const OrtShapeInferContext *, size_t, const OrtTensorTypeAndShapeInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtShapeInferContext*, nuint, OrtTensorTypeAndShapeInfo*, OrtStatus*> ShapeInferContext_SetOutputTypeShape;

        [NativeTypeName("OrtStatusPtr (*)(OrtTensorTypeAndShapeInfo *, const char **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, sbyte*, nuint, OrtStatus*> SetSymbolicDimensions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtOpAttr *, OrtOpAttrType, void *, size_t, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOpAttr*, OrtOpAttrType, void*, nuint, nuint*, OrtStatus*> ReadOpAttr;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, _Bool) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, byte, OrtStatus*> SetDeterministicCompute;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, void (*)(void *, size_t), size_t, size_t, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, delegate* unmanaged[Cdecl]<void*, nuint, void>, nuint, nuint, void*, OrtStatus*> KernelContext_ParallelFor;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte**, sbyte**, nuint, OrtStatus*> SessionOptionsAppendExecutionProvider_OpenVINO_V2;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, sbyte**, sbyte**, nuint, OrtStatus*> SessionOptionsAppendExecutionProvider_VitisAI;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, const OrtMemoryInfo *, size_t, void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, OrtMemoryInfo*, nuint, void**, OrtStatus*> KernelContext_GetScratchBuffer;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, OrtMemType, OrtAllocator **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, OrtMemType, OrtAllocator**, OrtStatus*> KernelInfoGetAllocator;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, const wchar_t *const *, char *const *, const size_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, ushort**, sbyte**, nuint*, nuint, OrtStatus*> AddExternalInitializersFromFilesInMemory;

        [NativeTypeName("OrtStatusPtr (*)(const wchar_t *, OrtAllocator *, OrtLoraAdapter **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<ushort*, OrtAllocator*, OrtLoraAdapter**, OrtStatus*> CreateLoraAdapter;

        [NativeTypeName("OrtStatusPtr (*)(const void *, size_t, OrtAllocator *, OrtLoraAdapter **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<void*, nuint, OrtAllocator*, OrtLoraAdapter**, OrtStatus*> CreateLoraAdapterFromArray;

        [NativeTypeName("void (*)(OrtLoraAdapter *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtLoraAdapter*, void> ReleaseLoraAdapter;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *, const OrtLoraAdapter *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, OrtLoraAdapter*, OrtStatus*> RunOptionsAddActiveLoraAdapter;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, sbyte**, sbyte**, nuint, OrtStatus*> SetEpDynamicOptions;

        [NativeTypeName("void (*)(OrtValueInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, void> ReleaseValueInfo;

        [NativeTypeName("void (*)(OrtNode *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, void> ReleaseNode;

        [NativeTypeName("void (*)(OrtGraph *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, void> ReleaseGraph;

        [NativeTypeName("void (*)(OrtModel *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModel*, void> ReleaseModel;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, sbyte**, OrtStatus*> GetValueInfoName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, const OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, OrtTypeInfo**, OrtStatus*> GetValueInfoTypeInfo;

        [NativeTypeName("const OrtModelEditorApi *(*)(void) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelEditorApi*> GetModelEditorApi;

        [NativeTypeName("OrtStatusPtr (*)(OrtAllocator *, void *, size_t, const int64_t *, size_t, ONNXTensorElementDataType, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, void*, nuint, long*, nuint, ONNXTensorElementDataType, OrtValue**, OrtStatus*> CreateTensorWithDataAndDeleterAsOrtValue;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, _Bool) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, byte, OrtStatus*> SessionOptionsSetLoadCancellationFlag;

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
        public delegate* unmanaged[Stdcall]<OrtEnv*, sbyte*, ushort*, OrtStatus*> RegisterExecutionProviderLibrary;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, sbyte*, OrtStatus*> UnregisterExecutionProviderLibrary;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const OrtEpDevice *const **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtEpDevice***, nuint*, OrtStatus*> GetEpDevices;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, OrtEnv *, const OrtEpDevice *const *, size_t, const char *const *, const char *const *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtEnv*, OrtEpDevice**, nuint, sbyte**, sbyte**, nuint, OrtStatus*> SessionOptionsAppendExecutionProvider_V2;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, OrtExecutionProviderDevicePolicy) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtExecutionProviderDevicePolicy, OrtStatus*> SessionOptionsSetEpSelectionPolicy;

        [NativeTypeName("OrtStatusPtr (*)(OrtSessionOptions *, EpSelectionDelegate, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, delegate* unmanaged[Stdcall]<OrtEpDevice**, nuint, OrtKeyValuePairs*, OrtKeyValuePairs*, OrtEpDevice**, nuint, nuint*, void*, OrtStatus*>, void*, OrtStatus*> SessionOptionsSetEpSelectionPolicyDelegate;

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
        public delegate* unmanaged[Stdcall]<OrtValue*, nuint*, OrtStatus*> GetTensorSizeInBytes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtAllocator *, OrtKeyValuePairs **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtAllocator*, OrtKeyValuePairs**, OrtStatus*> AllocatorGetStats;

        [NativeTypeName("OrtStatusPtr (*)(const char *, enum OrtMemoryInfoDeviceType, uint32_t, int32_t, enum OrtDeviceMemoryType, size_t, enum OrtAllocatorType, OrtMemoryInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, OrtMemoryInfoDeviceType, uint, int, OrtDeviceMemoryType, nuint, OrtAllocatorType, OrtMemoryInfo**, OrtStatus*> CreateMemoryInfo_V2;

        [NativeTypeName("OrtDeviceMemoryType (*)(const OrtMemoryInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, OrtDeviceMemoryType> MemoryInfoGetDeviceMemType;

        [NativeTypeName("uint32_t (*)(const OrtMemoryInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtMemoryInfo*, uint> MemoryInfoGetVendorId;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, const OrtNode **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, OrtNode**, nuint*, OrtStatus*> ValueInfo_GetValueProducer;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, nuint*, OrtStatus*> ValueInfo_GetValueNumConsumers;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, const OrtNode **, int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, OrtNode**, long*, nuint, OrtStatus*> ValueInfo_GetValueConsumers;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, const OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, OrtValue**, OrtStatus*> ValueInfo_GetInitializerValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, OrtExternalInitializerInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, OrtExternalInitializerInfo**, OrtStatus*> ValueInfo_GetExternalInitializerInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, bool*, OrtStatus*> ValueInfo_IsRequiredGraphInput;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, bool*, OrtStatus*> ValueInfo_IsOptionalGraphInput;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, bool*, OrtStatus*> ValueInfo_IsGraphOutput;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, bool*, OrtStatus*> ValueInfo_IsConstantInitializer;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValueInfo *, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValueInfo*, bool*, OrtStatus*> ValueInfo_IsFromOuterScope;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, sbyte**, OrtStatus*> Graph_GetName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const wchar_t **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, ushort**, OrtStatus*> Graph_GetModelPath;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, int64_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, long*, OrtStatus*> Graph_GetOnnxIRVersion;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, nuint*, OrtStatus*> Graph_GetNumOperatorSets;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const char **, int64_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, sbyte**, long*, nuint, OrtStatus*> Graph_GetOperatorSets;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, nuint*, OrtStatus*> Graph_GetNumInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtValueInfo**, nuint, OrtStatus*> Graph_GetInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, nuint*, OrtStatus*> Graph_GetNumOutputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtValueInfo**, nuint, OrtStatus*> Graph_GetOutputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, nuint*, OrtStatus*> Graph_GetNumInitializers;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtValueInfo**, nuint, OrtStatus*> Graph_GetInitializers;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, nuint*, OrtStatus*> Graph_GetNumNodes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const OrtNode **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtNode**, nuint, OrtStatus*> Graph_GetNodes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const OrtNode **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtNode**, OrtStatus*> Graph_GetParentNode;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, const OrtNode **, size_t, OrtGraph **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtNode**, nuint, OrtGraph**, OrtStatus*> Graph_GetGraphView;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, nuint*, OrtStatus*> Node_GetId;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, sbyte**, OrtStatus*> Node_GetName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, sbyte**, OrtStatus*> Node_GetOperatorType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, sbyte**, OrtStatus*> Node_GetDomain;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, int*, OrtStatus*> Node_GetSinceVersion;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, nuint*, OrtStatus*> Node_GetNumInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, OrtValueInfo**, nuint, OrtStatus*> Node_GetInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, nuint*, OrtStatus*> Node_GetNumOutputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, OrtValueInfo**, nuint, OrtStatus*> Node_GetOutputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, nuint*, OrtStatus*> Node_GetNumImplicitInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, OrtValueInfo**, nuint, OrtStatus*> Node_GetImplicitInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, nuint*, OrtStatus*> Node_GetNumAttributes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const OrtOpAttr **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, OrtOpAttr**, nuint, OrtStatus*> Node_GetAttributes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const char *, const OrtOpAttr **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, sbyte*, OrtOpAttr**, OrtStatus*> Node_GetAttributeByName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtOpAttr *, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOpAttr*, OrtValue**, OrtStatus*> OpAttr_GetTensorAttributeAsOrtValue;

        [NativeTypeName("OrtStatusPtr (*)(const OrtOpAttr *, OrtOpAttrType *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOpAttr*, OrtOpAttrType*, OrtStatus*> OpAttr_GetType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtOpAttr *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtOpAttr*, sbyte**, OrtStatus*> OpAttr_GetName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, nuint*, OrtStatus*> Node_GetNumSubgraphs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const OrtGraph **, size_t, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, OrtGraph**, nuint, sbyte**, OrtStatus*> Node_GetSubgraphs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const OrtGraph **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, OrtGraph**, OrtStatus*> Node_GetGraph;

        [NativeTypeName("OrtStatusPtr (*)(const OrtNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtNode*, sbyte**, OrtStatus*> Node_GetEpName;

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
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtEpDevice*, OrtDeviceMemoryType, OrtAllocatorType, OrtKeyValuePairs*, OrtAllocator**, OrtStatus*> CreateSharedAllocator;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const OrtMemoryInfo *, OrtAllocator **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtMemoryInfo*, OrtAllocator**, OrtStatus*> GetSharedAllocator;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const OrtEpDevice *, OrtDeviceMemoryType) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtEpDevice*, OrtDeviceMemoryType, OrtStatus*> ReleaseSharedAllocator;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, const void **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, void**, OrtStatus*> GetTensorData;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSessionOptions *, OrtKeyValuePairs **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, OrtKeyValuePairs**, OrtStatus*> GetSessionOptionsConfigEntries;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const OrtMemoryInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtMemoryInfo**, nuint, OrtStatus*> SessionGetMemoryInfoForInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const OrtMemoryInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtMemoryInfo**, nuint, OrtStatus*> SessionGetMemoryInfoForOutputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const OrtEpDevice **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtEpDevice**, nuint, OrtStatus*> SessionGetEpDeviceForInputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpDevice *, const OrtKeyValuePairs *, OrtSyncStream **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, OrtKeyValuePairs*, OrtSyncStream**, OrtStatus*> CreateSyncStreamForEpDevice;

        [NativeTypeName("void *(*)(OrtSyncStream *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSyncStream*, void*> SyncStream_GetHandle;

        [NativeTypeName("void (*)(OrtSyncStream *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSyncStream*, void> ReleaseSyncStream;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const OrtValue *const *, OrtValue *const *, OrtSyncStream *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtValue**, OrtValue**, OrtSyncStream*, nuint, OrtStatus*> CopyTensors;

        [NativeTypeName("OrtStatusPtr (*)(const OrtGraph *, OrtModelMetadata **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtModelMetadata**, OrtStatus*> Graph_GetModelMetadata;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpDevice *const *, size_t, const char *, OrtCompiledModelCompatibility *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice**, nuint, sbyte*, OrtCompiledModelCompatibility*, OrtStatus*> GetModelCompatibilityForEpDevices;

        [NativeTypeName("OrtStatusPtr (*)(const wchar_t *, int64_t, size_t, OrtExternalInitializerInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<ushort*, long, nuint, OrtExternalInitializerInfo**, OrtStatus*> CreateExternalInitializerInfo;

        [NativeTypeName("_Bool (*)(const OrtTensorTypeAndShapeInfo *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, byte> TensorTypeAndShape_HasShape;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, OrtKeyValuePairs **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, OrtKeyValuePairs**, OrtStatus*> KernelInfo_GetConfigEntries;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, nuint*, OrtStatus*> KernelInfo_GetOperatorDomain;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, char *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, nuint*, OrtStatus*> KernelInfo_GetOperatorType;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, int*, OrtStatus*> KernelInfo_GetOperatorSinceVersion;

        [NativeTypeName("const OrtInteropApi *(*)(void) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtInteropApi*> GetInteropApi;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const OrtEpDevice **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtEpDevice**, nuint, OrtStatus*> SessionGetEpDeviceForOutputs;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, nuint*, OrtStatus*> GetNumHardwareDevices;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const OrtHardwareDevice **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtHardwareDevice**, nuint, OrtStatus*> GetHardwareDevices;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const char *, const OrtHardwareDevice *, OrtDeviceEpIncompatibilityDetails **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, sbyte*, OrtHardwareDevice*, OrtDeviceEpIncompatibilityDetails**, OrtStatus*> GetHardwareDeviceEpIncompatibilityDetails;

        [NativeTypeName("OrtStatusPtr (*)(const OrtDeviceEpIncompatibilityDetails *, uint32_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDeviceEpIncompatibilityDetails*, uint*, OrtStatus*> DeviceEpIncompatibilityDetails_GetReasonsBitmask;

        [NativeTypeName("OrtStatusPtr (*)(const OrtDeviceEpIncompatibilityDetails *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDeviceEpIncompatibilityDetails*, sbyte**, OrtStatus*> DeviceEpIncompatibilityDetails_GetNotes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtDeviceEpIncompatibilityDetails *, int32_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDeviceEpIncompatibilityDetails*, int*, OrtStatus*> DeviceEpIncompatibilityDetails_GetErrorCode;

        [NativeTypeName("void (*)(OrtDeviceEpIncompatibilityDetails *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtDeviceEpIncompatibilityDetails*, void> ReleaseDeviceEpIncompatibilityDetails;

        [NativeTypeName("OrtStatusPtr (*)(const wchar_t *, const char *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<ushort*, sbyte*, OrtAllocator*, sbyte**, OrtStatus*> GetCompatibilityInfoFromModel;

        [NativeTypeName("OrtStatusPtr (*)(const void *, size_t, const char *, OrtAllocator *, char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<void*, nuint, sbyte*, OrtAllocator*, sbyte**, OrtStatus*> GetCompatibilityInfoFromModelBytes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnvCreationOptions *, OrtEnv **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnvCreationOptions*, OrtEnv**, OrtStatus*> CreateEnvWithOptions;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const OrtEpAssignedSubgraph *const **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtEpAssignedSubgraph***, nuint*, OrtStatus*> Session_GetEpGraphAssignmentInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpAssignedSubgraph *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpAssignedSubgraph*, sbyte**, OrtStatus*> EpAssignedSubgraph_GetEpName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpAssignedSubgraph *, const OrtEpAssignedNode *const **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpAssignedSubgraph*, OrtEpAssignedNode***, nuint*, OrtStatus*> EpAssignedSubgraph_GetNodes;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpAssignedNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpAssignedNode*, sbyte**, OrtStatus*> EpAssignedNode_GetName;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpAssignedNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpAssignedNode*, sbyte**, OrtStatus*> EpAssignedNode_GetDomain;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpAssignedNode *, const char **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpAssignedNode*, sbyte**, OrtStatus*> EpAssignedNode_GetOperatorType;

        [NativeTypeName("void (*)(OrtRunOptions *, OrtSyncStream *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, OrtSyncStream*, void> RunOptionsSetSyncStream;

        [NativeTypeName("OrtStatusPtr (*)(const OrtValue *, ONNXTensorElementDataType *, const int64_t **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtValue*, ONNXTensorElementDataType*, long**, nuint*, OrtStatus*> GetTensorElementTypeAndShapeDataReference;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, ushort*, OrtStatus*> RunOptionsEnableProfiling;

        [NativeTypeName("OrtStatusPtr (*)(OrtRunOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtRunOptions*, OrtStatus*> RunOptionsDisableProfiling;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelInfo *, const char *, OrtAllocator *, char ***, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelInfo*, sbyte*, OrtAllocator*, sbyte***, nuint*, OrtStatus*> KernelInfoGetAttributeArray_string;

        [NativeTypeName("OrtStatusPtr (*)(OrtEnv *, const OrtThreadPoolCallbacksConfig *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtThreadPoolCallbacksConfig*, OrtStatus*> SetPerSessionThreadPoolCallbacks;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSessionOptions *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, int*, OrtStatus*> GetMemPatternEnabled;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSessionOptions *, ExecutionMode *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSessionOptions*, ExecutionMode*, OrtStatus*> GetSessionExecutionMode;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, int) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, int, OrtStatus*> SessionReleaseCapturedGraph;

        [NativeTypeName("OrtExperimentalFnPtr (*)(const char *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, delegate* unmanaged[Stdcall]<void>> GetExperimentalFunction;

        [NativeTypeName("OrtStatusPtr (*)(const OrtKernelContext *, OrtSyncStream **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtKernelContext*, OrtSyncStream**, OrtStatus*> KernelContext_GetSyncStream;
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
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, OrtApi*, OrtKernelInfo*, void**, OrtStatus*> CreateKernelV2;

        [NativeTypeName("OrtStatusPtr (*)(void *, OrtKernelContext *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<void*, OrtKernelContext*, OrtStatus*> KernelComputeV2;

        [NativeTypeName("OrtStatusPtr (*)(const struct OrtCustomOp *, OrtShapeInferContext *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtCustomOp*, OrtShapeInferContext*, OrtStatus*> InferOutputShapeFn;

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
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, OrtTypeInfo**, OrtStatus*> CreateTensorTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTensorTypeAndShapeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTensorTypeAndShapeInfo*, OrtTypeInfo**, OrtStatus*> CreateSparseTensorTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(ONNXTensorElementDataType, const OrtTypeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<ONNXTensorElementDataType, OrtTypeInfo*, OrtTypeInfo**, OrtStatus*> CreateMapTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, OrtTypeInfo**, OrtStatus*> CreateSequenceTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const OrtTypeInfo *, OrtTypeInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtTypeInfo*, OrtTypeInfo**, OrtStatus*> CreateOptionalTypeInfo;

        [NativeTypeName("OrtStatusPtr (*)(const char *, const OrtTypeInfo *, OrtValueInfo **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, OrtTypeInfo*, OrtValueInfo**, OrtStatus*> CreateValueInfo;

        [NativeTypeName("OrtStatusPtr (*)(const char *, const char *, const char *, const char *const *, size_t, const char *const *, size_t, OrtOpAttr **, size_t, OrtNode **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte*, sbyte*, sbyte*, sbyte**, nuint, sbyte**, nuint, OrtOpAttr**, nuint, OrtNode**, OrtStatus*> CreateNode;

        [NativeTypeName("OrtStatusPtr (*)(OrtGraph **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph**, OrtStatus*> CreateGraph;

        [NativeTypeName("OrtStatusPtr (*)(OrtGraph *, OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtValueInfo**, nuint, OrtStatus*> SetGraphInputs;

        [NativeTypeName("OrtStatusPtr (*)(OrtGraph *, OrtValueInfo **, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtValueInfo**, nuint, OrtStatus*> SetGraphOutputs;

        [NativeTypeName("OrtStatusPtr (*)(OrtGraph *, const char *, OrtValue *, _Bool) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, sbyte*, OrtValue*, byte, OrtStatus*> AddInitializerToGraph;

        [NativeTypeName("OrtStatusPtr (*)(OrtGraph *, OrtNode *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtGraph*, OrtNode*, OrtStatus*> AddNodeToGraph;

        [NativeTypeName("OrtStatusPtr (*)(const char *const *, const int *, size_t, OrtModel **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<sbyte**, int*, nuint, OrtModel**, OrtStatus*> CreateModel;

        [NativeTypeName("OrtStatusPtr (*)(OrtModel *, OrtGraph *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModel*, OrtGraph*, OrtStatus*> AddGraphToModel;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const OrtModel *, const OrtSessionOptions *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtModel*, OrtSessionOptions*, OrtSession**, OrtStatus*> CreateSessionFromModel;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const wchar_t *, const OrtSessionOptions *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, ushort*, OrtSessionOptions*, OrtSession**, OrtStatus*> CreateModelEditorSession;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const void *, size_t, const OrtSessionOptions *, OrtSession **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, void*, nuint, OrtSessionOptions*, OrtSession**, OrtStatus*> CreateModelEditorSessionFromArray;

        [NativeTypeName("OrtStatusPtr (*)(const OrtSession *, const char *, int *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, sbyte*, int*, OrtStatus*> SessionGetOpsetForDomain;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, OrtModel *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtModel*, OrtStatus*> ApplyModelToModelEditorSession;

        [NativeTypeName("OrtStatusPtr (*)(OrtSession *, const OrtSessionOptions *, OrtPrepackedWeightsContainer *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtSession*, OrtSessionOptions*, OrtPrepackedWeightsContainer*, OrtStatus*> FinalizeModelEditorSession;
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
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtSessionOptions*, OrtModelCompilationOptions**, OrtStatus*> CreateModelCompilationOptionsFromSessionOptions;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, ushort*, OrtStatus*> ModelCompilationOptions_SetInputModelPath;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, const void *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, void*, nuint, OrtStatus*> ModelCompilationOptions_SetInputModelFromBuffer;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, ushort*, OrtStatus*> ModelCompilationOptions_SetOutputModelPath;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, const wchar_t *, size_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, ushort*, nuint, OrtStatus*> ModelCompilationOptions_SetOutputModelExternalInitializersFile;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, OrtAllocator *, void **, size_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, OrtAllocator*, void**, nuint*, OrtStatus*> ModelCompilationOptions_SetOutputModelBuffer;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, _Bool) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, byte, OrtStatus*> ModelCompilationOptions_SetEpContextEmbedMode;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEnv *, const OrtModelCompilationOptions *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEnv*, OrtModelCompilationOptions*, OrtStatus*> CompileModel;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, uint32_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, uint, OrtStatus*> ModelCompilationOptions_SetFlags;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, const wchar_t *, const wchar_t *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, ushort*, ushort*, OrtStatus*> ModelCompilationOptions_SetEpContextBinaryInformation;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, GraphOptimizationLevel) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, GraphOptimizationLevel, OrtStatus*> ModelCompilationOptions_SetGraphOptimizationLevel;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, OrtWriteBufferFunc, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, delegate* unmanaged[Stdcall]<void*, void*, nuint, OrtStatus*>, void*, OrtStatus*> ModelCompilationOptions_SetOutputModelWriteFunc;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, OrtGetInitializerLocationFunc, void *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, delegate* unmanaged[Stdcall]<void*, sbyte*, OrtValue*, OrtExternalInitializerInfo*, OrtExternalInitializerInfo**, OrtStatus*>, void*, OrtStatus*> ModelCompilationOptions_SetOutputModelGetInitializerLocationFunc;

        [NativeTypeName("OrtStatusPtr (*)(OrtModelCompilationOptions *, const OrtModel *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtModelCompilationOptions*, OrtModel*, OrtStatus*> ModelCompilationOptions_SetInputModel;
    }

    public unsafe partial struct OrtInteropApi
    {
        [NativeTypeName("OrtStatusPtr (*)(const OrtEpDevice *, OrtExternalResourceImporter **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, OrtExternalResourceImporter**, OrtStatus*> CreateExternalResourceImporterForDevice;

        [NativeTypeName("void (*)(OrtExternalResourceImporter *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, void> ReleaseExternalResourceImporter;

        [NativeTypeName("OrtStatusPtr (*)(const OrtExternalResourceImporter *, OrtExternalMemoryHandleType, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalMemoryHandleType, bool*, OrtStatus*> CanImportMemory;

        [NativeTypeName("OrtStatusPtr (*)(OrtExternalResourceImporter *, const OrtExternalMemoryDescriptor *, OrtExternalMemoryHandle **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalMemoryDescriptor*, OrtExternalMemoryHandle**, OrtStatus*> ImportMemory;

        [NativeTypeName("void (*)(OrtExternalMemoryHandle *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalMemoryHandle*, void> ReleaseExternalMemoryHandle;

        [NativeTypeName("OrtStatusPtr (*)(OrtExternalResourceImporter *, const OrtExternalMemoryHandle *, const OrtExternalTensorDescriptor *, OrtValue **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalMemoryHandle*, OrtExternalTensorDescriptor*, OrtValue**, OrtStatus*> CreateTensorFromMemory;

        [NativeTypeName("OrtStatusPtr (*)(const OrtExternalResourceImporter *, OrtExternalSemaphoreType, _Bool *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalSemaphoreType, bool*, OrtStatus*> CanImportSemaphore;

        [NativeTypeName("OrtStatusPtr (*)(OrtExternalResourceImporter *, const OrtExternalSemaphoreDescriptor *, OrtExternalSemaphoreHandle **) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalSemaphoreDescriptor*, OrtExternalSemaphoreHandle**, OrtStatus*> ImportSemaphore;

        [NativeTypeName("void (*)(OrtExternalSemaphoreHandle *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalSemaphoreHandle*, void> ReleaseExternalSemaphoreHandle;

        [NativeTypeName("OrtStatusPtr (*)(OrtExternalResourceImporter *, OrtExternalSemaphoreHandle *, OrtSyncStream *, uint64_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalSemaphoreHandle*, OrtSyncStream*, ulong, OrtStatus*> WaitSemaphore;

        [NativeTypeName("OrtStatusPtr (*)(OrtExternalResourceImporter *, OrtExternalSemaphoreHandle *, OrtSyncStream *, uint64_t) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtExternalResourceImporter*, OrtExternalSemaphoreHandle*, OrtSyncStream*, ulong, OrtStatus*> SignalSemaphore;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpDevice *, const OrtGraphicsInteropConfig *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, OrtGraphicsInteropConfig*, OrtStatus*> InitGraphicsInteropForEpDevice;

        [NativeTypeName("OrtStatusPtr (*)(const OrtEpDevice *) __attribute__((stdcall))")]
        public delegate* unmanaged[Stdcall]<OrtEpDevice*, OrtStatus*> DeinitGraphicsInteropForEpDevice;
    }

    internal static unsafe partial class NativeExports
    {
        [DllImport("onnxruntime", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: NativeTypeName("const OrtApiBase *")]
        public static extern OrtApiBase* OrtGetApiBase();

        [DllImport("onnxruntime", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: NativeTypeName("OrtStatusPtr")]
        public static extern OrtStatus* OrtSessionOptionsAppendExecutionProvider_CUDA(OrtSessionOptions* options, int device_id);

        [DllImport("onnxruntime", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: NativeTypeName("OrtStatusPtr")]
        public static extern OrtStatus* OrtSessionOptionsAppendExecutionProvider_ROCM(OrtSessionOptions* options, int device_id);

        [DllImport("onnxruntime", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: NativeTypeName("OrtStatusPtr")]
        public static extern OrtStatus* OrtSessionOptionsAppendExecutionProvider_MIGraphX(OrtSessionOptions* options, int device_id);

        [DllImport("onnxruntime", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: NativeTypeName("OrtStatusPtr")]
        public static extern OrtStatus* OrtSessionOptionsAppendExecutionProvider_Dnnl(OrtSessionOptions* options, int use_arena);

        [DllImport("onnxruntime", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: NativeTypeName("OrtStatusPtr")]
        public static extern OrtStatus* OrtSessionOptionsAppendExecutionProvider_Tensorrt(OrtSessionOptions* options, int device_id);
    }
}
