using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

/// <summary>Low-level bindings for ONNX Runtime C API version 28.</summary>
public static unsafe partial class Ort
{
    const uint ApiVersion = 28;

    internal static readonly OrtApi* Api = GetApi();

    static OrtApi* GetApi()
    {
        var apiBase = NativeExports.OrtGetApiBase();
        if (apiBase is null)
        {
            throw new InvalidOperationException("ONNX Runtime did not return an API base.");
        }
        var api = apiBase->GetApi(ApiVersion);
        if (api is null)
        {
            throw new NotSupportedException($"ONNX Runtime C API version {ApiVersion} is unavailable.");
        }
        return api;
    }

    internal static void ThrowIfError(OrtStatus* status)
    {
        if (status is null)
        {
            return;
        }
        try
        {
            throw new OrtException(Marshal.PtrToStringUTF8((IntPtr)GetErrorMessage(status)) ?? "Unknown ONNX Runtime error.");
        }
        finally
        {
            ReleaseStatus(status);
        }
    }

    internal static void ReleaseAllocatorValue(OrtAllocator* allocator, void* value)
    {
        var status = AllocatorFree(allocator, value);
        if (status is not null)
        {
            ReleaseStatus(status);
        }
    }

    internal static OrtEnv* CreateEnvironment(string logId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(logId);
        var utf8LogId = Utf8StringMarshaller.ConvertToUnmanaged(logId);
        try
        {
            OrtEnv* environment;
            ThrowIfError(CreateEnv(OrtLoggingLevel.ORT_LOGGING_LEVEL_WARNING, (sbyte*)utf8LogId, &environment));
            return environment;
        }
        finally
        {
            Utf8StringMarshaller.Free(utf8LogId);
        }
    }

    public static IReadOnlyList<string> GetAvailableExecutionProviders()
    {
        sbyte** providers;
        int providerCount;
        ThrowIfError(GetAvailableProviders(&providers, &providerCount));
        try
        {
            var result = new string[providerCount];
            for (var index = 0; index < result.Length; ++index)
            {
                result[index] = Marshal.PtrToStringUTF8((IntPtr)providers[index]) ??
                    throw new InvalidOperationException("ONNX Runtime returned a null execution provider name.");
            }
            return result;
        }
        finally
        {
            ReleaseAvailableProviders(providers, providerCount);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateStatus(OrtErrorCode code, sbyte* msg) => Api->CreateStatus(code, msg);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtErrorCode GetErrorCode(OrtStatus* status) => Api->GetErrorCode(status);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte* GetErrorMessage(OrtStatus* status) => Api->GetErrorMessage(status);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateEnv(OrtLoggingLevel log_severity_level, sbyte* logid, OrtEnv** @out) => Api->CreateEnv(log_severity_level, logid, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateEnvWithCustomLogger(delegate* unmanaged[Stdcall]<void*, OrtLoggingLevel, sbyte*, sbyte*, sbyte*, sbyte*, void> logging_function, void* logger_param, OrtLoggingLevel log_severity_level, sbyte* logid, OrtEnv** @out) => Api->CreateEnvWithCustomLogger(logging_function, logger_param, log_severity_level, logid, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* EnableTelemetryEvents(OrtEnv* env) => Api->EnableTelemetryEvents(env);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* DisableTelemetryEvents(OrtEnv* env) => Api->DisableTelemetryEvents(env);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateSession(OrtEnv* env, ushort* model_path, OrtSessionOptions* options, OrtSession** @out) => Api->CreateSession(env, model_path, options, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateSessionFromArray(OrtEnv* env, void* model_data, nuint model_data_length, OrtSessionOptions* options, OrtSession** @out) => Api->CreateSessionFromArray(env, model_data, model_data_length, options, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Run(OrtSession* session, OrtRunOptions* run_options, sbyte** input_names, OrtValue** inputs, nuint input_len, sbyte** output_names, nuint output_names_len, OrtValue** outputs) => Api->Run(session, run_options, input_names, inputs, input_len, output_names, output_names_len, outputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateSessionOptions(OrtSessionOptions** options) => Api->CreateSessionOptions(options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetOptimizedModelFilePath(OrtSessionOptions* options, ushort* optimized_model_filepath) => Api->SetOptimizedModelFilePath(options, optimized_model_filepath);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CloneSessionOptions(OrtSessionOptions* in_options, OrtSessionOptions** out_options) => Api->CloneSessionOptions(in_options, out_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetSessionExecutionMode(OrtSessionOptions* options, ExecutionMode execution_mode) => Api->SetSessionExecutionMode(options, execution_mode);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* EnableProfiling(OrtSessionOptions* options, ushort* profile_file_prefix) => Api->EnableProfiling(options, profile_file_prefix);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* DisableProfiling(OrtSessionOptions* options) => Api->DisableProfiling(options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* EnableMemPattern(OrtSessionOptions* options) => Api->EnableMemPattern(options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* DisableMemPattern(OrtSessionOptions* options) => Api->DisableMemPattern(options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* EnableCpuMemArena(OrtSessionOptions* options) => Api->EnableCpuMemArena(options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* DisableCpuMemArena(OrtSessionOptions* options) => Api->DisableCpuMemArena(options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetSessionLogId(OrtSessionOptions* options, sbyte* logid) => Api->SetSessionLogId(options, logid);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetSessionLogVerbosityLevel(OrtSessionOptions* options, int session_log_verbosity_level) => Api->SetSessionLogVerbosityLevel(options, session_log_verbosity_level);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetSessionLogSeverityLevel(OrtSessionOptions* options, int session_log_severity_level) => Api->SetSessionLogSeverityLevel(options, session_log_severity_level);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetSessionGraphOptimizationLevel(OrtSessionOptions* options, GraphOptimizationLevel graph_optimization_level) => Api->SetSessionGraphOptimizationLevel(options, graph_optimization_level);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetIntraOpNumThreads(OrtSessionOptions* options, int intra_op_num_threads) => Api->SetIntraOpNumThreads(options, intra_op_num_threads);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetInterOpNumThreads(OrtSessionOptions* options, int inter_op_num_threads) => Api->SetInterOpNumThreads(options, inter_op_num_threads);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateCustomOpDomain(sbyte* domain, OrtCustomOpDomain** @out) => Api->CreateCustomOpDomain(domain, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CustomOpDomain_Add(OrtCustomOpDomain* custom_op_domain, OrtCustomOp* op) => Api->CustomOpDomain_Add(custom_op_domain, op);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* AddCustomOpDomain(OrtSessionOptions* options, OrtCustomOpDomain* custom_op_domain) => Api->AddCustomOpDomain(options, custom_op_domain);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RegisterCustomOpsLibrary(OrtSessionOptions* options, sbyte* library_path, void** library_handle) => Api->RegisterCustomOpsLibrary(options, library_path, library_handle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetInputCount(OrtSession* session, nuint* @out) => Api->SessionGetInputCount(session, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetOutputCount(OrtSession* session, nuint* @out) => Api->SessionGetOutputCount(session, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetOverridableInitializerCount(OrtSession* session, nuint* @out) => Api->SessionGetOverridableInitializerCount(session, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetInputTypeInfo(OrtSession* session, nuint index, OrtTypeInfo** type_info) => Api->SessionGetInputTypeInfo(session, index, type_info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetOutputTypeInfo(OrtSession* session, nuint index, OrtTypeInfo** type_info) => Api->SessionGetOutputTypeInfo(session, index, type_info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetOverridableInitializerTypeInfo(OrtSession* session, nuint index, OrtTypeInfo** type_info) => Api->SessionGetOverridableInitializerTypeInfo(session, index, type_info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetInputName(OrtSession* session, nuint index, OrtAllocator* allocator, sbyte** value) => Api->SessionGetInputName(session, index, allocator, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetOutputName(OrtSession* session, nuint index, OrtAllocator* allocator, sbyte** value) => Api->SessionGetOutputName(session, index, allocator, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetOverridableInitializerName(OrtSession* session, nuint index, OrtAllocator* allocator, sbyte** value) => Api->SessionGetOverridableInitializerName(session, index, allocator, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateRunOptions(OrtRunOptions** @out) => Api->CreateRunOptions(@out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunOptionsSetRunLogVerbosityLevel(OrtRunOptions* options, int log_verbosity_level) => Api->RunOptionsSetRunLogVerbosityLevel(options, log_verbosity_level);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunOptionsSetRunLogSeverityLevel(OrtRunOptions* options, int log_severity_level) => Api->RunOptionsSetRunLogSeverityLevel(options, log_severity_level);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunOptionsSetRunTag(OrtRunOptions* options, sbyte* run_tag) => Api->RunOptionsSetRunTag(options, run_tag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunOptionsGetRunLogVerbosityLevel(OrtRunOptions* options, int* log_verbosity_level) => Api->RunOptionsGetRunLogVerbosityLevel(options, log_verbosity_level);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunOptionsGetRunLogSeverityLevel(OrtRunOptions* options, int* log_severity_level) => Api->RunOptionsGetRunLogSeverityLevel(options, log_severity_level);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunOptionsGetRunTag(OrtRunOptions* options, sbyte** run_tag) => Api->RunOptionsGetRunTag(options, run_tag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunOptionsSetTerminate(OrtRunOptions* options) => Api->RunOptionsSetTerminate(options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunOptionsUnsetTerminate(OrtRunOptions* options) => Api->RunOptionsUnsetTerminate(options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateTensorAsOrtValue(OrtAllocator* allocator, long* shape, nuint shape_len, ONNXTensorElementDataType type, OrtValue** @out) => Api->CreateTensorAsOrtValue(allocator, shape, shape_len, type, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateTensorWithDataAsOrtValue(OrtMemoryInfo* info, void* p_data, nuint p_data_len, long* shape, nuint shape_len, ONNXTensorElementDataType type, OrtValue** @out) => Api->CreateTensorWithDataAsOrtValue(info, p_data, p_data_len, shape, shape_len, type, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* IsTensor(OrtValue* value, int* @out) => Api->IsTensor(value, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetTensorMutableData(OrtValue* value, void** @out) => Api->GetTensorMutableData(value, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* FillStringTensor(OrtValue* value, sbyte** s, nuint s_len) => Api->FillStringTensor(value, s, s_len);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetStringTensorDataLength(OrtValue* value, nuint* len) => Api->GetStringTensorDataLength(value, len);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetStringTensorContent(OrtValue* value, void* s, nuint s_len, nuint* offsets, nuint offsets_len) => Api->GetStringTensorContent(value, s, s_len, offsets, offsets_len);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CastTypeInfoToTensorInfo(OrtTypeInfo* type_info, OrtTensorTypeAndShapeInfo** @out) => Api->CastTypeInfoToTensorInfo(type_info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetOnnxTypeFromTypeInfo(OrtTypeInfo* type_info, ONNXType* @out) => Api->GetOnnxTypeFromTypeInfo(type_info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateTensorTypeAndShapeInfo(OrtTensorTypeAndShapeInfo** @out) => Api->CreateTensorTypeAndShapeInfo(@out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetTensorElementType(OrtTensorTypeAndShapeInfo* info, ONNXTensorElementDataType type) => Api->SetTensorElementType(info, type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetDimensions(OrtTensorTypeAndShapeInfo* info, long* dim_values, nuint dim_count) => Api->SetDimensions(info, dim_values, dim_count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetTensorElementType(OrtTensorTypeAndShapeInfo* info, ONNXTensorElementDataType* @out) => Api->GetTensorElementType(info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetDimensionsCount(OrtTensorTypeAndShapeInfo* info, nuint* @out) => Api->GetDimensionsCount(info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetDimensions(OrtTensorTypeAndShapeInfo* info, long* dim_values, nuint dim_values_length) => Api->GetDimensions(info, dim_values, dim_values_length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetSymbolicDimensions(OrtTensorTypeAndShapeInfo* info, sbyte* dim_params, nuint dim_params_length) => Api->GetSymbolicDimensions(info, dim_params, dim_params_length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetTensorShapeElementCount(OrtTensorTypeAndShapeInfo* info, nuint* @out) => Api->GetTensorShapeElementCount(info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetTensorTypeAndShape(OrtValue* value, OrtTensorTypeAndShapeInfo** @out) => Api->GetTensorTypeAndShape(value, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetTypeInfo(OrtValue* value, OrtTypeInfo** @out) => Api->GetTypeInfo(value, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetValueType(OrtValue* value, ONNXType* @out) => Api->GetValueType(value, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateMemoryInfo(sbyte* name, OrtAllocatorType type, int id, OrtMemType mem_type, OrtMemoryInfo** @out) => Api->CreateMemoryInfo(name, type, id, mem_type, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateCpuMemoryInfo(OrtAllocatorType type, OrtMemType mem_type, OrtMemoryInfo** @out) => Api->CreateCpuMemoryInfo(type, mem_type, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CompareMemoryInfo(OrtMemoryInfo* info1, OrtMemoryInfo* info2, int* @out) => Api->CompareMemoryInfo(info1, info2, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* MemoryInfoGetName(OrtMemoryInfo* ptr, sbyte** @out) => Api->MemoryInfoGetName(ptr, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* MemoryInfoGetId(OrtMemoryInfo* ptr, int* @out) => Api->MemoryInfoGetId(ptr, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* MemoryInfoGetMemType(OrtMemoryInfo* ptr, OrtMemType* @out) => Api->MemoryInfoGetMemType(ptr, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* MemoryInfoGetType(OrtMemoryInfo* ptr, OrtAllocatorType* @out) => Api->MemoryInfoGetType(ptr, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* AllocatorAlloc(OrtAllocator* ort_allocator, nuint size, void** @out) => Api->AllocatorAlloc(ort_allocator, size, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* AllocatorFree(OrtAllocator* ort_allocator, void* p) => Api->AllocatorFree(ort_allocator, p);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* AllocatorGetInfo(OrtAllocator* ort_allocator, OrtMemoryInfo** @out) => Api->AllocatorGetInfo(ort_allocator, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetAllocatorWithDefaultOptions(OrtAllocator** @out) => Api->GetAllocatorWithDefaultOptions(@out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* AddFreeDimensionOverride(OrtSessionOptions* options, sbyte* dim_denotation, long dim_value) => Api->AddFreeDimensionOverride(options, dim_denotation, dim_value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetValue(OrtValue* value, int index, OrtAllocator* allocator, OrtValue** @out) => Api->GetValue(value, index, allocator, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetValueCount(OrtValue* value, nuint* @out) => Api->GetValueCount(value, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateValue(OrtValue** @in, nuint num_values, ONNXType value_type, OrtValue** @out) => Api->CreateValue(@in, num_values, value_type, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateOpaqueValue(sbyte* domain_name, sbyte* type_name, void* data_container, nuint data_container_size, OrtValue** @out) => Api->CreateOpaqueValue(domain_name, type_name, data_container, data_container_size, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetOpaqueValue(sbyte* domain_name, sbyte* type_name, OrtValue* @in, void* data_container, nuint data_container_size) => Api->GetOpaqueValue(domain_name, type_name, @in, data_container, data_container_size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfoGetAttribute_float(OrtKernelInfo* info, sbyte* name, float* @out) => Api->KernelInfoGetAttribute_float(info, name, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfoGetAttribute_int64(OrtKernelInfo* info, sbyte* name, long* @out) => Api->KernelInfoGetAttribute_int64(info, name, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfoGetAttribute_string(OrtKernelInfo* info, sbyte* name, sbyte* @out, nuint* size) => Api->KernelInfoGetAttribute_string(info, name, @out, size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelContext_GetInputCount(OrtKernelContext* context, nuint* @out) => Api->KernelContext_GetInputCount(context, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelContext_GetOutputCount(OrtKernelContext* context, nuint* @out) => Api->KernelContext_GetOutputCount(context, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelContext_GetInput(OrtKernelContext* context, nuint index, OrtValue** @out) => Api->KernelContext_GetInput(context, index, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelContext_GetOutput(OrtKernelContext* context, nuint index, long* dim_values, nuint dim_count, OrtValue** @out) => Api->KernelContext_GetOutput(context, index, dim_values, dim_count, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseEnv(OrtEnv* input) => Api->ReleaseEnv(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseStatus(OrtStatus* input) => Api->ReleaseStatus(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseMemoryInfo(OrtMemoryInfo* input) => Api->ReleaseMemoryInfo(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseSession(OrtSession* input) => Api->ReleaseSession(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseValue(OrtValue* input) => Api->ReleaseValue(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseRunOptions(OrtRunOptions* input) => Api->ReleaseRunOptions(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseTypeInfo(OrtTypeInfo* input) => Api->ReleaseTypeInfo(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseTensorTypeAndShapeInfo(OrtTensorTypeAndShapeInfo* input) => Api->ReleaseTensorTypeAndShapeInfo(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseSessionOptions(OrtSessionOptions* input) => Api->ReleaseSessionOptions(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseCustomOpDomain(OrtCustomOpDomain* input) => Api->ReleaseCustomOpDomain(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetDenotationFromTypeInfo(OrtTypeInfo* type_info, sbyte** denotation, nuint* len) => Api->GetDenotationFromTypeInfo(type_info, denotation, len);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CastTypeInfoToMapTypeInfo(OrtTypeInfo* type_info, OrtMapTypeInfo** @out) => Api->CastTypeInfoToMapTypeInfo(type_info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CastTypeInfoToSequenceTypeInfo(OrtTypeInfo* type_info, OrtSequenceTypeInfo** @out) => Api->CastTypeInfoToSequenceTypeInfo(type_info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetMapKeyType(OrtMapTypeInfo* map_type_info, ONNXTensorElementDataType* @out) => Api->GetMapKeyType(map_type_info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetMapValueType(OrtMapTypeInfo* map_type_info, OrtTypeInfo** type_info) => Api->GetMapValueType(map_type_info, type_info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetSequenceElementType(OrtSequenceTypeInfo* sequence_type_info, OrtTypeInfo** type_info) => Api->GetSequenceElementType(sequence_type_info, type_info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseMapTypeInfo(OrtMapTypeInfo* input) => Api->ReleaseMapTypeInfo(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseSequenceTypeInfo(OrtSequenceTypeInfo* input) => Api->ReleaseSequenceTypeInfo(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionEndProfiling(OrtSession* session, OrtAllocator* allocator, sbyte** @out) => Api->SessionEndProfiling(session, allocator, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetModelMetadata(OrtSession* session, OrtModelMetadata** @out) => Api->SessionGetModelMetadata(session, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ModelMetadataGetProducerName(OrtModelMetadata* model_metadata, OrtAllocator* allocator, sbyte** value) => Api->ModelMetadataGetProducerName(model_metadata, allocator, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ModelMetadataGetGraphName(OrtModelMetadata* model_metadata, OrtAllocator* allocator, sbyte** value) => Api->ModelMetadataGetGraphName(model_metadata, allocator, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ModelMetadataGetDomain(OrtModelMetadata* model_metadata, OrtAllocator* allocator, sbyte** value) => Api->ModelMetadataGetDomain(model_metadata, allocator, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ModelMetadataGetDescription(OrtModelMetadata* model_metadata, OrtAllocator* allocator, sbyte** value) => Api->ModelMetadataGetDescription(model_metadata, allocator, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ModelMetadataLookupCustomMetadataMap(OrtModelMetadata* model_metadata, OrtAllocator* allocator, sbyte* key, sbyte** value) => Api->ModelMetadataLookupCustomMetadataMap(model_metadata, allocator, key, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ModelMetadataGetVersion(OrtModelMetadata* model_metadata, long* value) => Api->ModelMetadataGetVersion(model_metadata, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseModelMetadata(OrtModelMetadata* input) => Api->ReleaseModelMetadata(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateEnvWithGlobalThreadPools(OrtLoggingLevel log_severity_level, sbyte* logid, OrtThreadingOptions* tp_options, OrtEnv** @out) => Api->CreateEnvWithGlobalThreadPools(log_severity_level, logid, tp_options, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* DisablePerSessionThreads(OrtSessionOptions* options) => Api->DisablePerSessionThreads(options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateThreadingOptions(OrtThreadingOptions** @out) => Api->CreateThreadingOptions(@out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseThreadingOptions(OrtThreadingOptions* input) => Api->ReleaseThreadingOptions(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ModelMetadataGetCustomMetadataMapKeys(OrtModelMetadata* model_metadata, OrtAllocator* allocator, sbyte*** keys, long* num_keys) => Api->ModelMetadataGetCustomMetadataMapKeys(model_metadata, allocator, keys, num_keys);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* AddFreeDimensionOverrideByName(OrtSessionOptions* options, sbyte* dim_name, long dim_value) => Api->AddFreeDimensionOverrideByName(options, dim_name, dim_value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetAvailableProviders(sbyte*** out_ptr, int* provider_length) => Api->GetAvailableProviders(out_ptr, provider_length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ReleaseAvailableProviders(sbyte** ptr, int providers_length) => Api->ReleaseAvailableProviders(ptr, providers_length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetStringTensorElementLength(OrtValue* value, nuint index, nuint* @out) => Api->GetStringTensorElementLength(value, index, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetStringTensorElement(OrtValue* value, nuint s_len, nuint index, void* s) => Api->GetStringTensorElement(value, s_len, index, s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* FillStringTensorElement(OrtValue* value, sbyte* s, nuint index) => Api->FillStringTensorElement(value, s, index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* AddSessionConfigEntry(OrtSessionOptions* options, sbyte* config_key, sbyte* config_value) => Api->AddSessionConfigEntry(options, config_key, config_value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateAllocator(OrtSession* session, OrtMemoryInfo* mem_info, OrtAllocator** @out) => Api->CreateAllocator(session, mem_info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseAllocator(OrtAllocator* input) => Api->ReleaseAllocator(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunWithBinding(OrtSession* session, OrtRunOptions* run_options, OrtIoBinding* binding_ptr) => Api->RunWithBinding(session, run_options, binding_ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateIoBinding(OrtSession* session, OrtIoBinding** @out) => Api->CreateIoBinding(session, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseIoBinding(OrtIoBinding* input) => Api->ReleaseIoBinding(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* BindInput(OrtIoBinding* binding_ptr, sbyte* name, OrtValue* val_ptr) => Api->BindInput(binding_ptr, name, val_ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* BindOutput(OrtIoBinding* binding_ptr, sbyte* name, OrtValue* val_ptr) => Api->BindOutput(binding_ptr, name, val_ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* BindOutputToDevice(OrtIoBinding* binding_ptr, sbyte* name, OrtMemoryInfo* mem_info_ptr) => Api->BindOutputToDevice(binding_ptr, name, mem_info_ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetBoundOutputNames(OrtIoBinding* binding_ptr, OrtAllocator* allocator, sbyte** buffer, nuint** lengths, nuint* count) => Api->GetBoundOutputNames(binding_ptr, allocator, buffer, lengths, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetBoundOutputValues(OrtIoBinding* binding_ptr, OrtAllocator* allocator, OrtValue*** output, nuint* output_count) => Api->GetBoundOutputValues(binding_ptr, allocator, output, output_count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ClearBoundInputs(OrtIoBinding* binding_ptr) => Api->ClearBoundInputs(binding_ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ClearBoundOutputs(OrtIoBinding* binding_ptr) => Api->ClearBoundOutputs(binding_ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* TensorAt(OrtValue* value, long* location_values, nuint location_values_count, void** @out) => Api->TensorAt(value, location_values, location_values_count, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateAndRegisterAllocator(OrtEnv* env, OrtMemoryInfo* mem_info, OrtArenaCfg* arena_cfg) => Api->CreateAndRegisterAllocator(env, mem_info, arena_cfg);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetLanguageProjection(OrtEnv* ort_env, OrtLanguageProjection projection) => Api->SetLanguageProjection(ort_env, projection);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetProfilingStartTimeNs(OrtSession* session, ulong* @out) => Api->SessionGetProfilingStartTimeNs(session, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetGlobalIntraOpNumThreads(OrtThreadingOptions* tp_options, int intra_op_num_threads) => Api->SetGlobalIntraOpNumThreads(tp_options, intra_op_num_threads);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetGlobalInterOpNumThreads(OrtThreadingOptions* tp_options, int inter_op_num_threads) => Api->SetGlobalInterOpNumThreads(tp_options, inter_op_num_threads);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetGlobalSpinControl(OrtThreadingOptions* tp_options, int allow_spinning) => Api->SetGlobalSpinControl(tp_options, allow_spinning);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* AddInitializer(OrtSessionOptions* options, sbyte* name, OrtValue* val) => Api->AddInitializer(options, name, val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateEnvWithCustomLoggerAndGlobalThreadPools(delegate* unmanaged[Stdcall]<void*, OrtLoggingLevel, sbyte*, sbyte*, sbyte*, sbyte*, void> logging_function, void* logger_param, OrtLoggingLevel log_severity_level, sbyte* logid, OrtThreadingOptions* tp_options, OrtEnv** @out) => Api->CreateEnvWithCustomLoggerAndGlobalThreadPools(logging_function, logger_param, log_severity_level, logid, tp_options, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider_CUDA(OrtSessionOptions* options, OrtCUDAProviderOptions* cuda_options) => Api->SessionOptionsAppendExecutionProvider_CUDA(options, cuda_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider_ROCM(OrtSessionOptions* options, OrtROCMProviderOptions* rocm_options) => Api->SessionOptionsAppendExecutionProvider_ROCM(options, rocm_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider_OpenVINO(OrtSessionOptions* options, OrtOpenVINOProviderOptions* provider_options) => Api->SessionOptionsAppendExecutionProvider_OpenVINO(options, provider_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetGlobalDenormalAsZero(OrtThreadingOptions* tp_options) => Api->SetGlobalDenormalAsZero(tp_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateArenaCfg(nuint max_mem, int arena_extend_strategy, int initial_chunk_size_bytes, int max_dead_bytes_per_chunk, OrtArenaCfg** @out) => Api->CreateArenaCfg(max_mem, arena_extend_strategy, initial_chunk_size_bytes, max_dead_bytes_per_chunk, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseArenaCfg(OrtArenaCfg* input) => Api->ReleaseArenaCfg(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ModelMetadataGetGraphDescription(OrtModelMetadata* model_metadata, OrtAllocator* allocator, sbyte** value) => Api->ModelMetadataGetGraphDescription(model_metadata, allocator, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider_TensorRT(OrtSessionOptions* options, OrtTensorRTProviderOptions* tensorrt_options) => Api->SessionOptionsAppendExecutionProvider_TensorRT(options, tensorrt_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetCurrentGpuDeviceId(int device_id) => Api->SetCurrentGpuDeviceId(device_id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetCurrentGpuDeviceId(int* device_id) => Api->GetCurrentGpuDeviceId(device_id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfoGetAttributeArray_float(OrtKernelInfo* info, sbyte* name, float* @out, nuint* size) => Api->KernelInfoGetAttributeArray_float(info, name, @out, size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfoGetAttributeArray_int64(OrtKernelInfo* info, sbyte* name, long* @out, nuint* size) => Api->KernelInfoGetAttributeArray_int64(info, name, @out, size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateArenaCfgV2(sbyte** arena_config_keys, nuint* arena_config_values, nuint num_keys, OrtArenaCfg** @out) => Api->CreateArenaCfgV2(arena_config_keys, arena_config_values, num_keys, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* AddRunConfigEntry(OrtRunOptions* options, sbyte* config_key, sbyte* config_value) => Api->AddRunConfigEntry(options, config_key, config_value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreatePrepackedWeightsContainer(OrtPrepackedWeightsContainer** @out) => Api->CreatePrepackedWeightsContainer(@out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleasePrepackedWeightsContainer(OrtPrepackedWeightsContainer* input) => Api->ReleasePrepackedWeightsContainer(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateSessionWithPrepackedWeightsContainer(OrtEnv* env, ushort* model_path, OrtSessionOptions* options, OrtPrepackedWeightsContainer* prepacked_weights_container, OrtSession** @out) => Api->CreateSessionWithPrepackedWeightsContainer(env, model_path, options, prepacked_weights_container, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateSessionFromArrayWithPrepackedWeightsContainer(OrtEnv* env, void* model_data, nuint model_data_length, OrtSessionOptions* options, OrtPrepackedWeightsContainer* prepacked_weights_container, OrtSession** @out) => Api->CreateSessionFromArrayWithPrepackedWeightsContainer(env, model_data, model_data_length, options, prepacked_weights_container, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider_TensorRT_V2(OrtSessionOptions* options, OrtTensorRTProviderOptionsV2* tensorrt_options) => Api->SessionOptionsAppendExecutionProvider_TensorRT_V2(options, tensorrt_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateTensorRTProviderOptions(OrtTensorRTProviderOptionsV2** @out) => Api->CreateTensorRTProviderOptions(@out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UpdateTensorRTProviderOptions(OrtTensorRTProviderOptionsV2* tensorrt_options, sbyte** provider_options_keys, sbyte** provider_options_values, nuint num_keys) => Api->UpdateTensorRTProviderOptions(tensorrt_options, provider_options_keys, provider_options_values, num_keys);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetTensorRTProviderOptionsAsString(OrtTensorRTProviderOptionsV2* tensorrt_options, OrtAllocator* allocator, sbyte** ptr) => Api->GetTensorRTProviderOptionsAsString(tensorrt_options, allocator, ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseTensorRTProviderOptions(OrtTensorRTProviderOptionsV2* input) => Api->ReleaseTensorRTProviderOptions(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* EnableOrtCustomOps(OrtSessionOptions* options) => Api->EnableOrtCustomOps(options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RegisterAllocator(OrtEnv* env, OrtAllocator* allocator) => Api->RegisterAllocator(env, allocator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UnregisterAllocator(OrtEnv* env, OrtMemoryInfo* mem_info) => Api->UnregisterAllocator(env, mem_info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* IsSparseTensor(OrtValue* value, int* @out) => Api->IsSparseTensor(value, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateSparseTensorAsOrtValue(OrtAllocator* allocator, long* dense_shape, nuint dense_shape_len, ONNXTensorElementDataType type, OrtValue** @out) => Api->CreateSparseTensorAsOrtValue(allocator, dense_shape, dense_shape_len, type, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* FillSparseTensorCoo(OrtValue* ort_value, OrtMemoryInfo* data_mem_info, long* values_shape, nuint values_shape_len, void* values, long* indices_data, nuint indices_num) => Api->FillSparseTensorCoo(ort_value, data_mem_info, values_shape, values_shape_len, values, indices_data, indices_num);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* FillSparseTensorCsr(OrtValue* ort_value, OrtMemoryInfo* data_mem_info, long* values_shape, nuint values_shape_len, void* values, long* inner_indices_data, nuint inner_indices_num, long* outer_indices_data, nuint outer_indices_num) => Api->FillSparseTensorCsr(ort_value, data_mem_info, values_shape, values_shape_len, values, inner_indices_data, inner_indices_num, outer_indices_data, outer_indices_num);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* FillSparseTensorBlockSparse(OrtValue* ort_value, OrtMemoryInfo* data_mem_info, long* values_shape, nuint values_shape_len, void* values, long* indices_shape_data, nuint indices_shape_len, int* indices_data) => Api->FillSparseTensorBlockSparse(ort_value, data_mem_info, values_shape, values_shape_len, values, indices_shape_data, indices_shape_len, indices_data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateSparseTensorWithValuesAsOrtValue(OrtMemoryInfo* info, void* p_data, long* dense_shape, nuint dense_shape_len, long* values_shape, nuint values_shape_len, ONNXTensorElementDataType type, OrtValue** @out) => Api->CreateSparseTensorWithValuesAsOrtValue(info, p_data, dense_shape, dense_shape_len, values_shape, values_shape_len, type, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UseCooIndices(OrtValue* ort_value, long* indices_data, nuint indices_num) => Api->UseCooIndices(ort_value, indices_data, indices_num);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UseCsrIndices(OrtValue* ort_value, long* inner_data, nuint inner_num, long* outer_data, nuint outer_num) => Api->UseCsrIndices(ort_value, inner_data, inner_num, outer_data, outer_num);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UseBlockSparseIndices(OrtValue* ort_value, long* indices_shape, nuint indices_shape_len, int* indices_data) => Api->UseBlockSparseIndices(ort_value, indices_shape, indices_shape_len, indices_data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetSparseTensorFormat(OrtValue* ort_value, OrtSparseFormat* @out) => Api->GetSparseTensorFormat(ort_value, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetSparseTensorValuesTypeAndShape(OrtValue* ort_value, OrtTensorTypeAndShapeInfo** @out) => Api->GetSparseTensorValuesTypeAndShape(ort_value, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetSparseTensorValues(OrtValue* ort_value, void** @out) => Api->GetSparseTensorValues(ort_value, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetSparseTensorIndicesTypeShape(OrtValue* ort_value, OrtSparseIndicesFormat indices_format, OrtTensorTypeAndShapeInfo** @out) => Api->GetSparseTensorIndicesTypeShape(ort_value, indices_format, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetSparseTensorIndices(OrtValue* ort_value, OrtSparseIndicesFormat indices_format, nuint* num_indices, void** indices) => Api->GetSparseTensorIndices(ort_value, indices_format, num_indices, indices);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* HasValue(OrtValue* value, int* @out) => Api->HasValue(value, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelContext_GetGPUComputeStream(OrtKernelContext* context, void** @out) => Api->KernelContext_GetGPUComputeStream(context, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetTensorMemoryInfo(OrtValue* value, OrtMemoryInfo** mem_info) => Api->GetTensorMemoryInfo(value, mem_info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetExecutionProviderApi(sbyte* provider_name, uint version, void** provider_api) => Api->GetExecutionProviderApi(provider_name, version, provider_api);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsSetCustomCreateThreadFn(OrtSessionOptions* options, delegate* unmanaged[Cdecl]<void*, delegate* unmanaged[Cdecl]<void*, void>, void*, OrtCustomHandleType*> ort_custom_create_thread_fn) => Api->SessionOptionsSetCustomCreateThreadFn(options, ort_custom_create_thread_fn);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsSetCustomThreadCreationOptions(OrtSessionOptions* options, void* ort_custom_thread_creation_options) => Api->SessionOptionsSetCustomThreadCreationOptions(options, ort_custom_thread_creation_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsSetCustomJoinThreadFn(OrtSessionOptions* options, delegate* unmanaged[Cdecl]<OrtCustomHandleType*, void> ort_custom_join_thread_fn) => Api->SessionOptionsSetCustomJoinThreadFn(options, ort_custom_join_thread_fn);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetGlobalCustomCreateThreadFn(OrtThreadingOptions* tp_options, delegate* unmanaged[Cdecl]<void*, delegate* unmanaged[Cdecl]<void*, void>, void*, OrtCustomHandleType*> ort_custom_create_thread_fn) => Api->SetGlobalCustomCreateThreadFn(tp_options, ort_custom_create_thread_fn);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetGlobalCustomThreadCreationOptions(OrtThreadingOptions* tp_options, void* ort_custom_thread_creation_options) => Api->SetGlobalCustomThreadCreationOptions(tp_options, ort_custom_thread_creation_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetGlobalCustomJoinThreadFn(OrtThreadingOptions* tp_options, delegate* unmanaged[Cdecl]<OrtCustomHandleType*, void> ort_custom_join_thread_fn) => Api->SetGlobalCustomJoinThreadFn(tp_options, ort_custom_join_thread_fn);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SynchronizeBoundInputs(OrtIoBinding* binding_ptr) => Api->SynchronizeBoundInputs(binding_ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SynchronizeBoundOutputs(OrtIoBinding* binding_ptr) => Api->SynchronizeBoundOutputs(binding_ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider_CUDA_V2(OrtSessionOptions* options, OrtCUDAProviderOptionsV2* cuda_options) => Api->SessionOptionsAppendExecutionProvider_CUDA_V2(options, cuda_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateCUDAProviderOptions(OrtCUDAProviderOptionsV2** @out) => Api->CreateCUDAProviderOptions(@out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UpdateCUDAProviderOptions(OrtCUDAProviderOptionsV2* cuda_options, sbyte** provider_options_keys, sbyte** provider_options_values, nuint num_keys) => Api->UpdateCUDAProviderOptions(cuda_options, provider_options_keys, provider_options_values, num_keys);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetCUDAProviderOptionsAsString(OrtCUDAProviderOptionsV2* cuda_options, OrtAllocator* allocator, sbyte** ptr) => Api->GetCUDAProviderOptionsAsString(cuda_options, allocator, ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseCUDAProviderOptions(OrtCUDAProviderOptionsV2* input) => Api->ReleaseCUDAProviderOptions(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider_MIGraphX(OrtSessionOptions* options, OrtMIGraphXProviderOptions* migraphx_options) => Api->SessionOptionsAppendExecutionProvider_MIGraphX(options, migraphx_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* AddExternalInitializers(OrtSessionOptions* options, sbyte** initializer_names, OrtValue** initializers, nuint num_initializers) => Api->AddExternalInitializers(options, initializer_names, initializers, num_initializers);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateOpAttr(sbyte* name, void* data, int len, OrtOpAttrType type, OrtOpAttr** op_attr) => Api->CreateOpAttr(name, data, len, type, op_attr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseOpAttr(OrtOpAttr* input) => Api->ReleaseOpAttr(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateOp(OrtKernelInfo* info, sbyte* op_name, sbyte* domain, int version, sbyte** type_constraint_names, ONNXTensorElementDataType* type_constraint_values, int type_constraint_count, OrtOpAttr** attr_values, int attr_count, int input_count, int output_count, OrtOp** ort_op) => Api->CreateOp(info, op_name, domain, version, type_constraint_names, type_constraint_values, type_constraint_count, attr_values, attr_count, input_count, output_count, ort_op);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* InvokeOp(OrtKernelContext* context, OrtOp* ort_op, OrtValue** input_values, int input_count, OrtValue** output_values, int output_count) => Api->InvokeOp(context, ort_op, input_values, input_count, output_values, output_count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseOp(OrtOp* input) => Api->ReleaseOp(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider(OrtSessionOptions* options, sbyte* provider_name, sbyte** provider_options_keys, sbyte** provider_options_values, nuint num_keys) => Api->SessionOptionsAppendExecutionProvider(options, provider_name, provider_options_keys, provider_options_values, num_keys);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CopyKernelInfo(OrtKernelInfo* info, OrtKernelInfo** info_copy) => Api->CopyKernelInfo(info, info_copy);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseKernelInfo(OrtKernelInfo* input) => Api->ReleaseKernelInfo(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtTrainingApi* GetTrainingApi(uint version) => Api->GetTrainingApi(version);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider_CANN(OrtSessionOptions* options, OrtCANNProviderOptions* cann_options) => Api->SessionOptionsAppendExecutionProvider_CANN(options, cann_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateCANNProviderOptions(OrtCANNProviderOptions** @out) => Api->CreateCANNProviderOptions(@out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UpdateCANNProviderOptions(OrtCANNProviderOptions* cann_options, sbyte** provider_options_keys, sbyte** provider_options_values, nuint num_keys) => Api->UpdateCANNProviderOptions(cann_options, provider_options_keys, provider_options_values, num_keys);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetCANNProviderOptionsAsString(OrtCANNProviderOptions* cann_options, OrtAllocator* allocator, sbyte** ptr) => Api->GetCANNProviderOptionsAsString(cann_options, allocator, ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseCANNProviderOptions(OrtCANNProviderOptions* input) => Api->ReleaseCANNProviderOptions(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MemoryInfoGetDeviceType(OrtMemoryInfo* ptr, OrtMemoryInfoDeviceType* @out) => Api->MemoryInfoGetDeviceType(ptr, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UpdateEnvWithCustomLogLevel(OrtEnv* ort_env, OrtLoggingLevel log_severity_level) => Api->UpdateEnvWithCustomLogLevel(ort_env, log_severity_level);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetGlobalIntraOpThreadAffinity(OrtThreadingOptions* tp_options, sbyte* affinity_string) => Api->SetGlobalIntraOpThreadAffinity(tp_options, affinity_string);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RegisterCustomOpsLibrary_V2(OrtSessionOptions* options, ushort* library_name) => Api->RegisterCustomOpsLibrary_V2(options, library_name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RegisterCustomOpsUsingFunction(OrtSessionOptions* options, sbyte* registration_func_name) => Api->RegisterCustomOpsUsingFunction(options, registration_func_name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfo_GetInputCount(OrtKernelInfo* info, nuint* @out) => Api->KernelInfo_GetInputCount(info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfo_GetOutputCount(OrtKernelInfo* info, nuint* @out) => Api->KernelInfo_GetOutputCount(info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfo_GetInputName(OrtKernelInfo* info, nuint index, sbyte* @out, nuint* size) => Api->KernelInfo_GetInputName(info, index, @out, size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfo_GetOutputName(OrtKernelInfo* info, nuint index, sbyte* @out, nuint* size) => Api->KernelInfo_GetOutputName(info, index, @out, size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfo_GetInputTypeInfo(OrtKernelInfo* info, nuint index, OrtTypeInfo** type_info) => Api->KernelInfo_GetInputTypeInfo(info, index, type_info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfo_GetOutputTypeInfo(OrtKernelInfo* info, nuint index, OrtTypeInfo** type_info) => Api->KernelInfo_GetOutputTypeInfo(info, index, type_info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfoGetAttribute_tensor(OrtKernelInfo* info, sbyte* name, OrtAllocator* allocator, OrtValue** @out) => Api->KernelInfoGetAttribute_tensor(info, name, allocator, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* HasSessionConfigEntry(OrtSessionOptions* options, sbyte* config_key, int* @out) => Api->HasSessionConfigEntry(options, config_key, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetSessionConfigEntry(OrtSessionOptions* options, sbyte* config_key, sbyte* config_value, nuint* size) => Api->GetSessionConfigEntry(options, config_key, config_value, size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider_Dnnl(OrtSessionOptions* options, OrtDnnlProviderOptions* dnnl_options) => Api->SessionOptionsAppendExecutionProvider_Dnnl(options, dnnl_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateDnnlProviderOptions(OrtDnnlProviderOptions** @out) => Api->CreateDnnlProviderOptions(@out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UpdateDnnlProviderOptions(OrtDnnlProviderOptions* dnnl_options, sbyte** provider_options_keys, sbyte** provider_options_values, nuint num_keys) => Api->UpdateDnnlProviderOptions(dnnl_options, provider_options_keys, provider_options_values, num_keys);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetDnnlProviderOptionsAsString(OrtDnnlProviderOptions* dnnl_options, OrtAllocator* allocator, sbyte** ptr) => Api->GetDnnlProviderOptionsAsString(dnnl_options, allocator, ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseDnnlProviderOptions(OrtDnnlProviderOptions* input) => Api->ReleaseDnnlProviderOptions(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfo_GetNodeName(OrtKernelInfo* info, sbyte* @out, nuint* size) => Api->KernelInfo_GetNodeName(info, @out, size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfo_GetLogger(OrtKernelInfo* info, OrtLogger** logger) => Api->KernelInfo_GetLogger(info, logger);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelContext_GetLogger(OrtKernelContext* context, OrtLogger** logger) => Api->KernelContext_GetLogger(context, logger);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Logger_LogMessage(OrtLogger* logger, OrtLoggingLevel log_severity_level, sbyte* message, ushort* file_path, int line_number, sbyte* func_name) => Api->Logger_LogMessage(logger, log_severity_level, message, file_path, line_number, func_name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Logger_GetLoggingSeverityLevel(OrtLogger* logger, OrtLoggingLevel* @out) => Api->Logger_GetLoggingSeverityLevel(logger, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfoGetConstantInput_tensor(OrtKernelInfo* info, nuint index, int* is_constant, OrtValue** @out) => Api->KernelInfoGetConstantInput_tensor(info, index, is_constant, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CastTypeInfoToOptionalTypeInfo(OrtTypeInfo* type_info, OrtOptionalTypeInfo** @out) => Api->CastTypeInfoToOptionalTypeInfo(type_info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetOptionalContainedTypeInfo(OrtOptionalTypeInfo* optional_type_info, OrtTypeInfo** @out) => Api->GetOptionalContainedTypeInfo(optional_type_info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetResizedStringTensorElementBuffer(OrtValue* value, nuint index, nuint length_in_bytes, sbyte** buffer) => Api->GetResizedStringTensorElementBuffer(value, index, length_in_bytes, buffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelContext_GetAllocator(OrtKernelContext* context, OrtMemoryInfo* mem_info, OrtAllocator** @out) => Api->KernelContext_GetAllocator(context, mem_info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte* GetBuildInfoString() => Api->GetBuildInfoString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateROCMProviderOptions(OrtROCMProviderOptions** @out) => Api->CreateROCMProviderOptions(@out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UpdateROCMProviderOptions(OrtROCMProviderOptions* rocm_options, sbyte** provider_options_keys, sbyte** provider_options_values, nuint num_keys) => Api->UpdateROCMProviderOptions(rocm_options, provider_options_keys, provider_options_values, num_keys);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetROCMProviderOptionsAsString(OrtROCMProviderOptions* rocm_options, OrtAllocator* allocator, sbyte** ptr) => Api->GetROCMProviderOptionsAsString(rocm_options, allocator, ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseROCMProviderOptions(OrtROCMProviderOptions* input) => Api->ReleaseROCMProviderOptions(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateAndRegisterAllocatorV2(OrtEnv* env, sbyte* provider_type, OrtMemoryInfo* mem_info, OrtArenaCfg* arena_cfg, sbyte** provider_options_keys, sbyte** provider_options_values, nuint num_keys) => Api->CreateAndRegisterAllocatorV2(env, provider_type, mem_info, arena_cfg, provider_options_keys, provider_options_values, num_keys);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunAsync(OrtSession* session, OrtRunOptions* run_options, sbyte** input_names, OrtValue** input, nuint input_len, sbyte** output_names, nuint output_names_len, OrtValue** output, delegate* unmanaged[Cdecl]<void*, OrtValue**, nuint, OrtStatus*, void> run_async_callback, void* user_data) => Api->RunAsync(session, run_options, input_names, input, input_len, output_names, output_names_len, output, run_async_callback, user_data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UpdateTensorRTProviderOptionsWithValue(OrtTensorRTProviderOptionsV2* tensorrt_options, sbyte* key, void* value) => Api->UpdateTensorRTProviderOptionsWithValue(tensorrt_options, key, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetTensorRTProviderOptionsByName(OrtTensorRTProviderOptionsV2* tensorrt_options, sbyte* key, void** ptr) => Api->GetTensorRTProviderOptionsByName(tensorrt_options, key, ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UpdateCUDAProviderOptionsWithValue(OrtCUDAProviderOptionsV2* cuda_options, sbyte* key, void* value) => Api->UpdateCUDAProviderOptionsWithValue(cuda_options, key, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetCUDAProviderOptionsByName(OrtCUDAProviderOptionsV2* cuda_options, sbyte* key, void** ptr) => Api->GetCUDAProviderOptionsByName(cuda_options, key, ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelContext_GetResource(OrtKernelContext* context, int resource_version, int resource_id, void** resource) => Api->KernelContext_GetResource(context, resource_version, resource_id, resource);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetUserLoggingFunction(OrtSessionOptions* options, delegate* unmanaged[Stdcall]<void*, OrtLoggingLevel, sbyte*, sbyte*, sbyte*, sbyte*, void> user_logging_function, void* user_logging_param) => Api->SetUserLoggingFunction(options, user_logging_function, user_logging_param);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ShapeInferContext_GetInputCount(OrtShapeInferContext* context, nuint* @out) => Api->ShapeInferContext_GetInputCount(context, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ShapeInferContext_GetInputTypeShape(OrtShapeInferContext* context, nuint index, OrtTensorTypeAndShapeInfo** info) => Api->ShapeInferContext_GetInputTypeShape(context, index, info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ShapeInferContext_GetAttribute(OrtShapeInferContext* context, sbyte* attr_name, OrtOpAttr** attr) => Api->ShapeInferContext_GetAttribute(context, attr_name, attr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ShapeInferContext_SetOutputTypeShape(OrtShapeInferContext* context, nuint index, OrtTensorTypeAndShapeInfo* info) => Api->ShapeInferContext_SetOutputTypeShape(context, index, info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetSymbolicDimensions(OrtTensorTypeAndShapeInfo* info, sbyte* dim_params, nuint dim_params_length) => Api->SetSymbolicDimensions(info, dim_params, dim_params_length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ReadOpAttr(OrtOpAttr* op_attr, OrtOpAttrType type, void* data, nuint len, nuint* @out) => Api->ReadOpAttr(op_attr, type, data, len, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetDeterministicCompute(OrtSessionOptions* options, byte value) => Api->SetDeterministicCompute(options, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelContext_ParallelFor(OrtKernelContext* context, delegate* unmanaged[Cdecl]<void*, nuint, void> fn, nuint total, nuint num_batch, void* usr_data) => Api->KernelContext_ParallelFor(context, fn, total, num_batch, usr_data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider_OpenVINO_V2(OrtSessionOptions* options, sbyte** provider_options_keys, sbyte** provider_options_values, nuint num_keys) => Api->SessionOptionsAppendExecutionProvider_OpenVINO_V2(options, provider_options_keys, provider_options_values, num_keys);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider_VitisAI(OrtSessionOptions* options, sbyte** provider_options_keys, sbyte** provider_options_values, nuint num_keys) => Api->SessionOptionsAppendExecutionProvider_VitisAI(options, provider_options_keys, provider_options_values, num_keys);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelContext_GetScratchBuffer(OrtKernelContext* context, OrtMemoryInfo* mem_info, nuint count_or_bytes, void** @out) => Api->KernelContext_GetScratchBuffer(context, mem_info, count_or_bytes, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfoGetAllocator(OrtKernelInfo* info, OrtMemType mem_type, OrtAllocator** @out) => Api->KernelInfoGetAllocator(info, mem_type, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* AddExternalInitializersFromFilesInMemory(OrtSessionOptions* options, ushort** external_initializer_file_names, sbyte** external_initializer_file_buffer_array, nuint* external_initializer_file_lengths, nuint num_external_initializer_files) => Api->AddExternalInitializersFromFilesInMemory(options, external_initializer_file_names, external_initializer_file_buffer_array, external_initializer_file_lengths, num_external_initializer_files);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateLoraAdapter(ushort* adapter_file_path, OrtAllocator* allocator, OrtLoraAdapter** @out) => Api->CreateLoraAdapter(adapter_file_path, allocator, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateLoraAdapterFromArray(void* bytes, nuint num_bytes, OrtAllocator* allocator, OrtLoraAdapter** @out) => Api->CreateLoraAdapterFromArray(bytes, num_bytes, allocator, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseLoraAdapter(OrtLoraAdapter* input) => Api->ReleaseLoraAdapter(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunOptionsAddActiveLoraAdapter(OrtRunOptions* options, OrtLoraAdapter* adapter) => Api->RunOptionsAddActiveLoraAdapter(options, adapter);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetEpDynamicOptions(OrtSession* sess, sbyte** keys, sbyte** values, nuint kv_len) => Api->SetEpDynamicOptions(sess, keys, values, kv_len);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseValueInfo(OrtValueInfo* input) => Api->ReleaseValueInfo(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseNode(OrtNode* input) => Api->ReleaseNode(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseGraph(OrtGraph* input) => Api->ReleaseGraph(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseModel(OrtModel* input) => Api->ReleaseModel(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetValueInfoName(OrtValueInfo* value_info, sbyte** name) => Api->GetValueInfoName(value_info, name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetValueInfoTypeInfo(OrtValueInfo* value_info, OrtTypeInfo** type_info) => Api->GetValueInfoTypeInfo(value_info, type_info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtModelEditorApi* GetModelEditorApi() => Api->GetModelEditorApi();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateTensorWithDataAndDeleterAsOrtValue(OrtAllocator* deleter, void* p_data, nuint p_data_len, long* shape, nuint shape_len, ONNXTensorElementDataType type, OrtValue** @out) => Api->CreateTensorWithDataAndDeleterAsOrtValue(deleter, p_data, p_data_len, shape, shape_len, type, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsSetLoadCancellationFlag(OrtSessionOptions* options, byte cancel) => Api->SessionOptionsSetLoadCancellationFlag(options, cancel);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtCompileApi* GetCompileApi() => Api->GetCompileApi();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateKeyValuePairs(OrtKeyValuePairs** @out) => Api->CreateKeyValuePairs(@out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddKeyValuePair(OrtKeyValuePairs* kvps, sbyte* key, sbyte* value) => Api->AddKeyValuePair(kvps, key, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte* GetKeyValue(OrtKeyValuePairs* kvps, sbyte* key) => Api->GetKeyValue(kvps, key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetKeyValuePairs(OrtKeyValuePairs* kvps, sbyte*** keys, sbyte*** values, nuint* num_entries) => Api->GetKeyValuePairs(kvps, keys, values, num_entries);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveKeyValuePair(OrtKeyValuePairs* kvps, sbyte* key) => Api->RemoveKeyValuePair(kvps, key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseKeyValuePairs(OrtKeyValuePairs* input) => Api->ReleaseKeyValuePairs(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RegisterExecutionProviderLibrary(OrtEnv* env, sbyte* registration_name, ushort* path) => Api->RegisterExecutionProviderLibrary(env, registration_name, path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* UnregisterExecutionProviderLibrary(OrtEnv* env, sbyte* registration_name) => Api->UnregisterExecutionProviderLibrary(env, registration_name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetEpDevices(OrtEnv* env, OrtEpDevice*** ep_devices, nuint* num_ep_devices) => Api->GetEpDevices(env, ep_devices, num_ep_devices);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsAppendExecutionProvider_V2(OrtSessionOptions* session_options, OrtEnv* env, OrtEpDevice** ep_devices, nuint num_ep_devices, sbyte** ep_option_keys, sbyte** ep_option_vals, nuint num_ep_options) => Api->SessionOptionsAppendExecutionProvider_V2(session_options, env, ep_devices, num_ep_devices, ep_option_keys, ep_option_vals, num_ep_options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsSetEpSelectionPolicy(OrtSessionOptions* session_options, OrtExecutionProviderDevicePolicy policy) => Api->SessionOptionsSetEpSelectionPolicy(session_options, policy);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionOptionsSetEpSelectionPolicyDelegate(OrtSessionOptions* session_options, delegate* unmanaged[Stdcall]<OrtEpDevice**, nuint, OrtKeyValuePairs*, OrtKeyValuePairs*, OrtEpDevice**, nuint, nuint*, void*, OrtStatus*> @delegate, void* delegate_state) => Api->SessionOptionsSetEpSelectionPolicyDelegate(session_options, @delegate, delegate_state);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtHardwareDeviceType HardwareDevice_Type(OrtHardwareDevice* device) => Api->HardwareDevice_Type(device);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint HardwareDevice_VendorId(OrtHardwareDevice* device) => Api->HardwareDevice_VendorId(device);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte* HardwareDevice_Vendor(OrtHardwareDevice* device) => Api->HardwareDevice_Vendor(device);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint HardwareDevice_DeviceId(OrtHardwareDevice* device) => Api->HardwareDevice_DeviceId(device);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtKeyValuePairs* HardwareDevice_Metadata(OrtHardwareDevice* device) => Api->HardwareDevice_Metadata(device);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte* EpDevice_EpName(OrtEpDevice* ep_device) => Api->EpDevice_EpName(ep_device);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte* EpDevice_EpVendor(OrtEpDevice* ep_device) => Api->EpDevice_EpVendor(ep_device);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtKeyValuePairs* EpDevice_EpMetadata(OrtEpDevice* ep_device) => Api->EpDevice_EpMetadata(ep_device);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtKeyValuePairs* EpDevice_EpOptions(OrtEpDevice* ep_device) => Api->EpDevice_EpOptions(ep_device);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtHardwareDevice* EpDevice_Device(OrtEpDevice* ep_device) => Api->EpDevice_Device(ep_device);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtEpApi* GetEpApi() => Api->GetEpApi();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetTensorSizeInBytes(OrtValue* ort_value, nuint* size) => Api->GetTensorSizeInBytes(ort_value, size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* AllocatorGetStats(OrtAllocator* ort_allocator, OrtKeyValuePairs** @out) => Api->AllocatorGetStats(ort_allocator, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateMemoryInfo_V2(sbyte* name, OrtMemoryInfoDeviceType device_type, uint vendor_id, int device_id, OrtDeviceMemoryType mem_type, nuint alignment, OrtAllocatorType allocator_type, OrtMemoryInfo** @out) => Api->CreateMemoryInfo_V2(name, device_type, vendor_id, device_id, mem_type, alignment, allocator_type, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtDeviceMemoryType MemoryInfoGetDeviceMemType(OrtMemoryInfo* ptr) => Api->MemoryInfoGetDeviceMemType(ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint MemoryInfoGetVendorId(OrtMemoryInfo* ptr) => Api->MemoryInfoGetVendorId(ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ValueInfo_GetValueProducer(OrtValueInfo* value_info, OrtNode** producer_node, nuint* producer_output_index) => Api->ValueInfo_GetValueProducer(value_info, producer_node, producer_output_index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ValueInfo_GetValueNumConsumers(OrtValueInfo* value_info, nuint* num_consumers) => Api->ValueInfo_GetValueNumConsumers(value_info, num_consumers);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ValueInfo_GetValueConsumers(OrtValueInfo* value_info, OrtNode** nodes, long* input_indices, nuint num_consumers) => Api->ValueInfo_GetValueConsumers(value_info, nodes, input_indices, num_consumers);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ValueInfo_GetInitializerValue(OrtValueInfo* value_info, OrtValue** initializer_value) => Api->ValueInfo_GetInitializerValue(value_info, initializer_value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ValueInfo_GetExternalInitializerInfo(OrtValueInfo* value_info, OrtExternalInitializerInfo** info) => Api->ValueInfo_GetExternalInitializerInfo(value_info, info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ValueInfo_IsRequiredGraphInput(OrtValueInfo* value_info, bool* is_required_graph_input) => Api->ValueInfo_IsRequiredGraphInput(value_info, is_required_graph_input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ValueInfo_IsOptionalGraphInput(OrtValueInfo* value_info, bool* is_optional_graph_input) => Api->ValueInfo_IsOptionalGraphInput(value_info, is_optional_graph_input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ValueInfo_IsGraphOutput(OrtValueInfo* value_info, bool* is_graph_output) => Api->ValueInfo_IsGraphOutput(value_info, is_graph_output);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ValueInfo_IsConstantInitializer(OrtValueInfo* value_info, bool* is_constant_initializer) => Api->ValueInfo_IsConstantInitializer(value_info, is_constant_initializer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ValueInfo_IsFromOuterScope(OrtValueInfo* value_info, bool* is_from_outer_scope) => Api->ValueInfo_IsFromOuterScope(value_info, is_from_outer_scope);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetName(OrtGraph* graph, sbyte** graph_name) => Api->Graph_GetName(graph, graph_name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetModelPath(OrtGraph* graph, ushort** model_path) => Api->Graph_GetModelPath(graph, model_path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetOnnxIRVersion(OrtGraph* graph, long* onnx_ir_version) => Api->Graph_GetOnnxIRVersion(graph, onnx_ir_version);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetNumOperatorSets(OrtGraph* graph, nuint* num_operator_sets) => Api->Graph_GetNumOperatorSets(graph, num_operator_sets);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetOperatorSets(OrtGraph* graph, sbyte** domains, long* opset_versions, nuint num_operator_sets) => Api->Graph_GetOperatorSets(graph, domains, opset_versions, num_operator_sets);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetNumInputs(OrtGraph* graph, nuint* num_inputs) => Api->Graph_GetNumInputs(graph, num_inputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetInputs(OrtGraph* graph, OrtValueInfo** inputs, nuint num_inputs) => Api->Graph_GetInputs(graph, inputs, num_inputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetNumOutputs(OrtGraph* graph, nuint* num_outputs) => Api->Graph_GetNumOutputs(graph, num_outputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetOutputs(OrtGraph* graph, OrtValueInfo** outputs, nuint num_outputs) => Api->Graph_GetOutputs(graph, outputs, num_outputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetNumInitializers(OrtGraph* graph, nuint* num_initializers) => Api->Graph_GetNumInitializers(graph, num_initializers);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetInitializers(OrtGraph* graph, OrtValueInfo** initializers, nuint num_initializers) => Api->Graph_GetInitializers(graph, initializers, num_initializers);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetNumNodes(OrtGraph* graph, nuint* num_nodes) => Api->Graph_GetNumNodes(graph, num_nodes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetNodes(OrtGraph* graph, OrtNode** nodes, nuint num_nodes) => Api->Graph_GetNodes(graph, nodes, num_nodes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetParentNode(OrtGraph* graph, OrtNode** node) => Api->Graph_GetParentNode(graph, node);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetGraphView(OrtGraph* src_graph, OrtNode** nodes, nuint num_nodes, OrtGraph** dst_graph) => Api->Graph_GetGraphView(src_graph, nodes, num_nodes, dst_graph);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetId(OrtNode* node, nuint* node_id) => Api->Node_GetId(node, node_id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetName(OrtNode* node, sbyte** node_name) => Api->Node_GetName(node, node_name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetOperatorType(OrtNode* node, sbyte** operator_type) => Api->Node_GetOperatorType(node, operator_type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetDomain(OrtNode* node, sbyte** domain_name) => Api->Node_GetDomain(node, domain_name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetSinceVersion(OrtNode* node, int* since_version) => Api->Node_GetSinceVersion(node, since_version);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetNumInputs(OrtNode* node, nuint* num_inputs) => Api->Node_GetNumInputs(node, num_inputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetInputs(OrtNode* node, OrtValueInfo** inputs, nuint num_inputs) => Api->Node_GetInputs(node, inputs, num_inputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetNumOutputs(OrtNode* node, nuint* num_outputs) => Api->Node_GetNumOutputs(node, num_outputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetOutputs(OrtNode* node, OrtValueInfo** outputs, nuint num_outputs) => Api->Node_GetOutputs(node, outputs, num_outputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetNumImplicitInputs(OrtNode* node, nuint* num_implicit_inputs) => Api->Node_GetNumImplicitInputs(node, num_implicit_inputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetImplicitInputs(OrtNode* node, OrtValueInfo** implicit_inputs, nuint num_implicit_inputs) => Api->Node_GetImplicitInputs(node, implicit_inputs, num_implicit_inputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetNumAttributes(OrtNode* node, nuint* num_attributes) => Api->Node_GetNumAttributes(node, num_attributes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetAttributes(OrtNode* node, OrtOpAttr** attributes, nuint num_attributes) => Api->Node_GetAttributes(node, attributes, num_attributes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetAttributeByName(OrtNode* node, sbyte* attribute_name, OrtOpAttr** attribute) => Api->Node_GetAttributeByName(node, attribute_name, attribute);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* OpAttr_GetTensorAttributeAsOrtValue(OrtOpAttr* attribute, OrtValue** attr_tensor) => Api->OpAttr_GetTensorAttributeAsOrtValue(attribute, attr_tensor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* OpAttr_GetType(OrtOpAttr* attribute, OrtOpAttrType* type) => Api->OpAttr_GetType(attribute, type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* OpAttr_GetName(OrtOpAttr* attribute, sbyte** name) => Api->OpAttr_GetName(attribute, name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetNumSubgraphs(OrtNode* node, nuint* num_subgraphs) => Api->Node_GetNumSubgraphs(node, num_subgraphs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetSubgraphs(OrtNode* node, OrtGraph** subgraphs, nuint num_subgraphs, sbyte** attribute_names) => Api->Node_GetSubgraphs(node, subgraphs, num_subgraphs, attribute_names);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetGraph(OrtNode* node, OrtGraph** graph) => Api->Node_GetGraph(node, graph);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Node_GetEpName(OrtNode* node, sbyte** @out) => Api->Node_GetEpName(node, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseExternalInitializerInfo(OrtExternalInitializerInfo* input) => Api->ReleaseExternalInitializerInfo(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort* ExternalInitializerInfo_GetFilePath(OrtExternalInitializerInfo* info) => Api->ExternalInitializerInfo_GetFilePath(info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ExternalInitializerInfo_GetFileOffset(OrtExternalInitializerInfo* info) => Api->ExternalInitializerInfo_GetFileOffset(info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint ExternalInitializerInfo_GetByteSize(OrtExternalInitializerInfo* info) => Api->ExternalInitializerInfo_GetByteSize(info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte* GetRunConfigEntry(OrtRunOptions* options, sbyte* config_key) => Api->GetRunConfigEntry(options, config_key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtMemoryInfo* EpDevice_MemoryInfo(OrtEpDevice* ep_device, OrtDeviceMemoryType memory_type) => Api->EpDevice_MemoryInfo(ep_device, memory_type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateSharedAllocator(OrtEnv* env, OrtEpDevice* ep_device, OrtDeviceMemoryType mem_type, OrtAllocatorType allocator_type, OrtKeyValuePairs* allocator_options, OrtAllocator** allocator) => Api->CreateSharedAllocator(env, ep_device, mem_type, allocator_type, allocator_options, allocator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetSharedAllocator(OrtEnv* env, OrtMemoryInfo* mem_info, OrtAllocator** allocator) => Api->GetSharedAllocator(env, mem_info, allocator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* ReleaseSharedAllocator(OrtEnv* env, OrtEpDevice* ep_device, OrtDeviceMemoryType mem_type) => Api->ReleaseSharedAllocator(env, ep_device, mem_type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetTensorData(OrtValue* value, void** @out) => Api->GetTensorData(value, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetSessionOptionsConfigEntries(OrtSessionOptions* options, OrtKeyValuePairs** @out) => Api->GetSessionOptionsConfigEntries(options, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetMemoryInfoForInputs(OrtSession* session, OrtMemoryInfo** inputs_memory_info, nuint num_inputs) => Api->SessionGetMemoryInfoForInputs(session, inputs_memory_info, num_inputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetMemoryInfoForOutputs(OrtSession* session, OrtMemoryInfo** outputs_memory_info, nuint num_outputs) => Api->SessionGetMemoryInfoForOutputs(session, outputs_memory_info, num_outputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetEpDeviceForInputs(OrtSession* session, OrtEpDevice** inputs_ep_devices, nuint num_inputs) => Api->SessionGetEpDeviceForInputs(session, inputs_ep_devices, num_inputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateSyncStreamForEpDevice(OrtEpDevice* ep_device, OrtKeyValuePairs* stream_options, OrtSyncStream** stream) => Api->CreateSyncStreamForEpDevice(ep_device, stream_options, stream);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* SyncStream_GetHandle(OrtSyncStream* stream) => Api->SyncStream_GetHandle(stream);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseSyncStream(OrtSyncStream* input) => Api->ReleaseSyncStream(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CopyTensors(OrtEnv* env, OrtValue** src_tensors, OrtValue** dst_tensors, OrtSyncStream* stream, nuint num_tensors) => Api->CopyTensors(env, src_tensors, dst_tensors, stream, num_tensors);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Graph_GetModelMetadata(OrtGraph* graph, OrtModelMetadata** @out) => Api->Graph_GetModelMetadata(graph, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetModelCompatibilityForEpDevices(OrtEpDevice** ep_devices, nuint num_ep_devices, sbyte* compatibility_info, OrtCompiledModelCompatibility* out_status) => Api->GetModelCompatibilityForEpDevices(ep_devices, num_ep_devices, compatibility_info, out_status);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateExternalInitializerInfo(ushort* filepath, long file_offset, nuint byte_size, OrtExternalInitializerInfo** @out) => Api->CreateExternalInitializerInfo(filepath, file_offset, byte_size, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte TensorTypeAndShape_HasShape(OrtTensorTypeAndShapeInfo* info) => Api->TensorTypeAndShape_HasShape(info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfo_GetConfigEntries(OrtKernelInfo* info, OrtKeyValuePairs** @out) => Api->KernelInfo_GetConfigEntries(info, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfo_GetOperatorDomain(OrtKernelInfo* info, sbyte* @out, nuint* size) => Api->KernelInfo_GetOperatorDomain(info, @out, size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfo_GetOperatorType(OrtKernelInfo* info, sbyte* @out, nuint* size) => Api->KernelInfo_GetOperatorType(info, @out, size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfo_GetOperatorSinceVersion(OrtKernelInfo* info, int* since_version) => Api->KernelInfo_GetOperatorSinceVersion(info, since_version);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtInteropApi* GetInteropApi() => Api->GetInteropApi();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionGetEpDeviceForOutputs(OrtSession* session, OrtEpDevice** outputs_ep_devices, nuint num_outputs) => Api->SessionGetEpDeviceForOutputs(session, outputs_ep_devices, num_outputs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetNumHardwareDevices(OrtEnv* env, nuint* num_devices) => Api->GetNumHardwareDevices(env, num_devices);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetHardwareDevices(OrtEnv* env, OrtHardwareDevice** devices, nuint num_devices) => Api->GetHardwareDevices(env, devices, num_devices);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetHardwareDeviceEpIncompatibilityDetails(OrtEnv* env, sbyte* ep_name, OrtHardwareDevice* hw, OrtDeviceEpIncompatibilityDetails** details) => Api->GetHardwareDeviceEpIncompatibilityDetails(env, ep_name, hw, details);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* DeviceEpIncompatibilityDetails_GetReasonsBitmask(OrtDeviceEpIncompatibilityDetails* details, uint* reasons_bitmask) => Api->DeviceEpIncompatibilityDetails_GetReasonsBitmask(details, reasons_bitmask);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* DeviceEpIncompatibilityDetails_GetNotes(OrtDeviceEpIncompatibilityDetails* details, sbyte** notes) => Api->DeviceEpIncompatibilityDetails_GetNotes(details, notes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* DeviceEpIncompatibilityDetails_GetErrorCode(OrtDeviceEpIncompatibilityDetails* details, int* error_code) => Api->DeviceEpIncompatibilityDetails_GetErrorCode(details, error_code);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReleaseDeviceEpIncompatibilityDetails(OrtDeviceEpIncompatibilityDetails* input) => Api->ReleaseDeviceEpIncompatibilityDetails(input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetCompatibilityInfoFromModel(ushort* model_path, sbyte* ep_type, OrtAllocator* allocator, sbyte** compatibility_info) => Api->GetCompatibilityInfoFromModel(model_path, ep_type, allocator, compatibility_info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetCompatibilityInfoFromModelBytes(void* model_data, nuint model_data_length, sbyte* ep_type, OrtAllocator* allocator, sbyte** compatibility_info) => Api->GetCompatibilityInfoFromModelBytes(model_data, model_data_length, ep_type, allocator, compatibility_info);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* CreateEnvWithOptions(OrtEnvCreationOptions* options, OrtEnv** @out) => Api->CreateEnvWithOptions(options, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* Session_GetEpGraphAssignmentInfo(OrtSession* session, OrtEpAssignedSubgraph*** ep_subgraphs, nuint* num_ep_subgraphs) => Api->Session_GetEpGraphAssignmentInfo(session, ep_subgraphs, num_ep_subgraphs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* EpAssignedSubgraph_GetEpName(OrtEpAssignedSubgraph* ep_subgraph, sbyte** @out) => Api->EpAssignedSubgraph_GetEpName(ep_subgraph, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* EpAssignedSubgraph_GetNodes(OrtEpAssignedSubgraph* ep_subgraph, OrtEpAssignedNode*** ep_nodes, nuint* num_ep_nodes) => Api->EpAssignedSubgraph_GetNodes(ep_subgraph, ep_nodes, num_ep_nodes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* EpAssignedNode_GetName(OrtEpAssignedNode* ep_node, sbyte** @out) => Api->EpAssignedNode_GetName(ep_node, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* EpAssignedNode_GetDomain(OrtEpAssignedNode* ep_node, sbyte** @out) => Api->EpAssignedNode_GetDomain(ep_node, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* EpAssignedNode_GetOperatorType(OrtEpAssignedNode* ep_node, sbyte** @out) => Api->EpAssignedNode_GetOperatorType(ep_node, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RunOptionsSetSyncStream(OrtRunOptions* options, OrtSyncStream* sync_stream) => Api->RunOptionsSetSyncStream(options, sync_stream);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetTensorElementTypeAndShapeDataReference(OrtValue* value, ONNXTensorElementDataType* elem_type, long** shape_data, nuint* shape_data_count) => Api->GetTensorElementTypeAndShapeDataReference(value, elem_type, shape_data, shape_data_count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunOptionsEnableProfiling(OrtRunOptions* options, ushort* profile_file_prefix) => Api->RunOptionsEnableProfiling(options, profile_file_prefix);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* RunOptionsDisableProfiling(OrtRunOptions* options) => Api->RunOptionsDisableProfiling(options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelInfoGetAttributeArray_string(OrtKernelInfo* info, sbyte* name, OrtAllocator* allocator, sbyte*** @out, nuint* size) => Api->KernelInfoGetAttributeArray_string(info, name, allocator, @out, size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SetPerSessionThreadPoolCallbacks(OrtEnv* env, OrtThreadPoolCallbacksConfig* config) => Api->SetPerSessionThreadPoolCallbacks(env, config);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetMemPatternEnabled(OrtSessionOptions* options, int* @out) => Api->GetMemPatternEnabled(options, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* GetSessionExecutionMode(OrtSessionOptions* options, ExecutionMode* @out) => Api->GetSessionExecutionMode(options, @out);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* SessionReleaseCapturedGraph(OrtSession* session, int graph_annotation_id) => Api->SessionReleaseCapturedGraph(session, graph_annotation_id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static delegate* unmanaged[Stdcall]<void> GetExperimentalFunction(sbyte* name) => Api->GetExperimentalFunction(name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrtStatus* KernelContext_GetSyncStream(OrtKernelContext* context, OrtSyncStream** @out) => Api->KernelContext_GetSyncStream(context, @out);
}
