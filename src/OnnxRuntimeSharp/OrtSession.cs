using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtSession : SafeHandle
{
    readonly OrtEnvironment _environment;
    readonly OrtTensorInfo[] _inputs = [];
    readonly OrtTensorInfo[] _outputs = [];
    readonly OrtTensorInfo[] _overridableInitializers = [];
    readonly OrtModelMetadata _modelMetadata = null!;
    bool _environmentReferenceAdded;

    public OrtSession(OrtEnvironment environment, ReadOnlySpan<byte> model, OrtSessionOptions? options = null)
        : base(IntPtr.Zero, ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(environment);
        if (model.IsEmpty)
        {
            Throws.ThrowModelDataEmpty();
        }

        _environment = environment;
        var ownsOptions = options is null;
        options ??= new OrtSessionOptions();
        var optionsReferenceAdded = false;
        try
        {
            environment.DangerousAddRef(ref _environmentReferenceAdded);
            options.DangerousAddRef(ref optionsReferenceAdded);
            fixed (byte* modelPointer = model)
            {
                Ort.OrtSession* session;
                Ort.Ok(Ort.CreateSessionFromArray(
                    (Ort.OrtEnv*)environment.DangerousGetHandle(),
                    modelPointer,
                    (nuint)model.Length,
                    (Ort.OrtSessionOptions*)options.DangerousGetHandle(),
                    &session));
                SetHandle((IntPtr)session);
            }

            _inputs = GetTensorInfos(TensorInfoKind.Input);
            _outputs = GetTensorInfos(TensorInfoKind.Output);
            _overridableInitializers = GetOverridableInitializerInfos();
            _modelMetadata = GetModelMetadata();
        }
        catch
        {
            Dispose();
            ReleaseEnvironmentReference();
            throw;
        }
        finally
        {
            if (optionsReferenceAdded)
            {
                options.DangerousRelease();
            }
            if (ownsOptions)
            {
                options.Dispose();
            }
        }
    }

    public OrtSession(OrtEnvironment environment, string modelPath, OrtSessionOptions? options = null)
        : base(IntPtr.Zero, ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentException.ThrowIfNullOrWhiteSpace(modelPath);

        _environment = environment;
        var ownsOptions = options is null;
        options ??= new OrtSessionOptions();
        var optionsReferenceAdded = false;
        try
        {
            environment.DangerousAddRef(ref _environmentReferenceAdded);
            options.DangerousAddRef(ref optionsReferenceAdded);
            Ort.OrtSession* session;
            if (OperatingSystem.IsWindows())
            {
                fixed (char* pathPointer = modelPath)
                {
                    Ort.Ok(Ort.CreateSession(
                        environment.Pointer,
                        (ushort*)pathPointer,
                        options.Pointer,
                        &session));
                }
            }
            else
            {
                var utf8Path = Utf8StringMarshaller.ConvertToUnmanaged(modelPath);
                try
                {
                    Ort.Ok(Ort.CreateSession(
                        environment.Pointer,
                        (ushort*)utf8Path,
                        options.Pointer,
                        &session));
                }
                finally
                {
                    Utf8StringMarshaller.Free(utf8Path);
                }
            }
            SetHandle((IntPtr)session);

            _inputs = GetTensorInfos(TensorInfoKind.Input);
            _outputs = GetTensorInfos(TensorInfoKind.Output);
            _overridableInitializers = GetOverridableInitializerInfos();
            _modelMetadata = GetModelMetadata();
        }
        catch
        {
            Dispose();
            ReleaseEnvironmentReference();
            throw;
        }
        finally
        {
            if (optionsReferenceAdded)
            {
                options.DangerousRelease();
            }
            if (ownsOptions)
            {
                options.Dispose();
            }
        }
    }

    public IReadOnlyList<OrtTensorInfo> Inputs => _inputs;

    public IReadOnlyList<OrtTensorInfo> Outputs => _outputs;

    public IReadOnlyList<OrtTensorInfo> OverridableInitializers => _overridableInitializers;

    public OrtModelMetadata ModelMetadata => _modelMetadata;

    internal Ort.OrtSession* Pointer => (Ort.OrtSession*)handle;

    public OrtIoBinding CreateIoBinding()
    {
        ThrowIfDisposed();
        return new OrtIoBinding(this);
    }

    public void Run(OrtIoBinding binding, OrtRunOptions? runOptions = null)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(binding);
        if (!ReferenceEquals(binding.Session, this))
        {
            Throws.ThrowIoBindingSessionMismatch();
        }

        var sessionReferenceAdded = false;
        var bindingReferenceAdded = false;
        var runOptionsReferenceAdded = false;
        try
        {
            DangerousAddRef(ref sessionReferenceAdded);
            binding.DangerousAddRef(ref bindingReferenceAdded);
            runOptions?.DangerousAddRef(ref runOptionsReferenceAdded);
            Ort.Ok(Ort.RunWithBinding(Pointer, runOptions?.Pointer, binding.Pointer));
        }
        finally
        {
            if (runOptionsReferenceAdded)
            {
                runOptions!.DangerousRelease();
            }
            if (bindingReferenceAdded)
            {
                binding.DangerousRelease();
            }
            if (sessionReferenceAdded)
            {
                DangerousRelease();
            }
        }
    }

    public string InputName => _inputs[0].Name;

    public string OutputName => _outputs[0].Name;

    public ReadOnlyMemory<long> InputDimensions => _inputs[0].Dimensions;

    public ReadOnlyMemory<long> OutputDimensions => _outputs[0].Dimensions;

    public OrtValueBinding CreateInputBinding<T>(int index, OrtTensor<T> value)
        where T : unmanaged
    {
        ThrowIfDisposed();
        return CreateBinding(_inputs, index, value);
    }

    public OrtValueBinding CreateOutputBinding<T>(int index, OrtTensor<T> value)
        where T : unmanaged
    {
        ThrowIfDisposed();
        return CreateBinding(_outputs, index, value);
    }

    public void Run<TInput, TOutput>(
        OrtTensor<TInput> input,
        OrtTensor<TOutput> output,
        OrtRunOptions? runOptions = null)
        where TInput : unmanaged
        where TOutput : unmanaged
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        if (_inputs.Length != 1 || _outputs.Length != 1)
        {
            Throws.ThrowSingleInputOutputModelRequired();
        }

        var sessionReferenceAdded = false;
        var inputReferenceAdded = false;
        var outputReferenceAdded = false;
        var runOptionsReferenceAdded = false;
        try
        {
            DangerousAddRef(ref sessionReferenceAdded);
            input.DangerousAddRef(ref inputReferenceAdded);
            output.DangerousAddRef(ref outputReferenceAdded);
            runOptions?.DangerousAddRef(ref runOptionsReferenceAdded);
            var inputName = _inputs[0].NamePointer;
            var outputName = _outputs[0].NamePointer;
            var inputValue = (Ort.OrtValue*)input.DangerousGetHandle();
            var outputValue = (Ort.OrtValue*)output.DangerousGetHandle();
            Ort.Ok(Ort.Run(
                (Ort.OrtSession*)handle,
                runOptions?.Pointer,
                &inputName,
                &inputValue,
                1,
                &outputName,
                1,
                &outputValue));
        }
        finally
        {
            if (runOptionsReferenceAdded)
            {
                runOptions!.DangerousRelease();
            }
            if (outputReferenceAdded)
            {
                output.DangerousRelease();
            }
            if (inputReferenceAdded)
            {
                input.DangerousRelease();
            }
            if (sessionReferenceAdded)
            {
                DangerousRelease();
            }
        }
    }

    public void Run(
        ReadOnlySpan<OrtValueBinding> inputs,
        ReadOnlySpan<OrtValueBinding> outputs,
        OrtRunOptions? runOptions = null)
    {
        ThrowIfDisposed();
        if (inputs.Length != _inputs.Length)
        {
            Throws.ThrowInputBindingCountMismatch(_inputs.Length, inputs.Length);
        }
        if ((uint)(outputs.Length - 1) >= (uint)_outputs.Length)
        {
            Throws.ThrowOutputBindingCountMismatch(_outputs.Length, outputs.Length);
        }

        var inputNames = stackalloc sbyte*[inputs.Length];
        var inputValues = stackalloc Ort.OrtValue*[inputs.Length];
        var outputNames = stackalloc sbyte*[outputs.Length];
        var outputValues = stackalloc Ort.OrtValue*[outputs.Length];
        var sessionReferenceAdded = false;
        var runOptionsReferenceAdded = false;
        var referencedInputCount = 0;
        var referencedOutputCount = 0;
        try
        {
            DangerousAddRef(ref sessionReferenceAdded);
            runOptions?.DangerousAddRef(ref runOptionsReferenceAdded);
            for (var index = 0; index < inputs.Length; ++index)
            {
                ValidateInputBinding(inputs[index], _inputs, index);
                var referenceAdded = false;
                inputs[index].Value.DangerousAddRef(ref referenceAdded);
                if (!referenceAdded)
                {
                    Throws.ThrowBindingValueDisposed(inputs[index].Value.GetType().Name);
                }
                ++referencedInputCount;
                inputNames[index] = inputs[index].NamePointer;
                inputValues[index] = inputs[index].ValuePointer;
            }
            for (var index = 0; index < outputs.Length; ++index)
            {
                ValidateOutputBinding(outputs[index], index, outputs);
                var referenceAdded = false;
                outputs[index].Value.DangerousAddRef(ref referenceAdded);
                if (!referenceAdded)
                {
                    Throws.ThrowBindingValueDisposed(outputs[index].Value.GetType().Name);
                }
                ++referencedOutputCount;
                outputNames[index] = outputs[index].NamePointer;
                outputValues[index] = outputs[index].ValuePointer;
            }

            Ort.Ok(Ort.Run(
                (Ort.OrtSession*)handle,
                runOptions?.Pointer,
                inputNames,
                inputValues,
                (nuint)inputs.Length,
                outputNames,
                (nuint)outputs.Length,
                outputValues));
        }
        finally
        {
            if (runOptionsReferenceAdded)
            {
                runOptions!.DangerousRelease();
            }
            for (var index = referencedOutputCount - 1; index >= 0; --index)
            {
                outputs[index].Value.DangerousRelease();
            }
            for (var index = referencedInputCount - 1; index >= 0; --index)
            {
                inputs[index].Value.DangerousRelease();
            }
            if (sessionReferenceAdded)
            {
                DangerousRelease();
            }
        }
    }

    public OrtValue[] Run(ReadOnlySpan<OrtValueBinding> inputs, OrtRunOptions? runOptions = null)
    {
        ThrowIfDisposed();
        if (inputs.Length != _inputs.Length)
        {
            Throws.ThrowInputBindingCountMismatch(_inputs.Length, inputs.Length);
        }

        var inputNames = stackalloc sbyte*[inputs.Length];
        var inputValues = stackalloc Ort.OrtValue*[inputs.Length];
        var outputNames = stackalloc sbyte*[_outputs.Length];
        var outputValues = stackalloc Ort.OrtValue*[_outputs.Length];
        for (var index = 0; index < _outputs.Length; ++index)
        {
            outputNames[index] = _outputs[index].NamePointer;
            outputValues[index] = null;
        }

        var sessionReferenceAdded = false;
        var runOptionsReferenceAdded = false;
        var referencedInputCount = 0;
        var results = new OrtValue[_outputs.Length];
        var initializedResultCount = 0;
        try
        {
            DangerousAddRef(ref sessionReferenceAdded);
            runOptions?.DangerousAddRef(ref runOptionsReferenceAdded);
            for (var index = 0; index < inputs.Length; ++index)
            {
                ValidateInputBinding(inputs[index], _inputs, index);
                var referenceAdded = false;
                inputs[index].Value.DangerousAddRef(ref referenceAdded);
                ++referencedInputCount;
                inputNames[index] = inputs[index].NamePointer;
                inputValues[index] = inputs[index].ValuePointer;
            }

            Ort.Ok(Ort.Run(
                (Ort.OrtSession*)handle,
                runOptions?.Pointer,
                inputNames,
                inputValues,
                (nuint)inputs.Length,
                outputNames,
                (nuint)_outputs.Length,
                outputValues));

            for (var index = 0; index < results.Length; ++index)
            {
                var value = outputValues[index];
                outputValues[index] = null;
                results[index] = new OrtValue(value);
                ++initializedResultCount;
            }
            return results;
        }
        catch
        {
            for (var index = 0; index < initializedResultCount; ++index)
            {
                results[index].Dispose();
            }
            for (var index = 0; index < _outputs.Length; ++index)
            {
                if (outputValues[index] is not null)
                {
                    Ort.ReleaseValue(outputValues[index]);
                }
            }
            throw;
        }
        finally
        {
            for (var index = referencedInputCount - 1; index >= 0; --index)
            {
                inputs[index].Value.DangerousRelease();
            }
            if (runOptionsReferenceAdded)
            {
                runOptions!.DangerousRelease();
            }
            if (sessionReferenceAdded)
            {
                DangerousRelease();
            }
        }
    }

    public string EndProfiling()
    {
        ThrowIfDisposed();
        var sessionReferenceAdded = false;
        try
        {
            DangerousAddRef(ref sessionReferenceAdded);
            Ort.OrtAllocator* allocator;
            Ort.Ok(Ort.GetAllocatorWithDefaultOptions(&allocator));
            sbyte* profilePath;
            Ort.Ok(Ort.SessionEndProfiling((Ort.OrtSession*)handle, allocator, &profilePath));
            try
            {
                return Marshal.PtrToStringUTF8((IntPtr)profilePath) ?? string.Empty;
            }
            finally
            {
                Ort.Ok(Ort.AllocatorFree(allocator, profilePath));
            }
        }
        finally
        {
            if (sessionReferenceAdded)
            {
                DangerousRelease();
            }
        }
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        foreach (var input in _inputs)
        {
            input.Dispose();
        }
        foreach (var output in _outputs)
        {
            output.Dispose();
        }
        foreach (var initializer in _overridableInitializers)
        {
            initializer.Dispose();
        }
        Ort.ReleaseSession((Ort.OrtSession*)handle);
        ReleaseEnvironmentReference();
        return true;
    }

    OrtValueBinding CreateBinding<T>(OrtTensorInfo[] infos, int index, OrtTensor<T> value)
        where T : unmanaged
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, infos.Length);
        ArgumentNullException.ThrowIfNull(value);
        if (infos[index].ElementType != value.ElementType)
        {
            Throws.ThrowTensorBindingTypeMismatch(infos[index].Name, infos[index].ElementType, value.ElementType);
        }

        return new OrtValueBinding(this, infos[index], value);
    }

    void ValidateInputBinding(
        OrtValueBinding binding,
        OrtTensorInfo[] expectedInfos,
        int index)
    {
        if (!ReferenceEquals(binding.Session, this) || !ReferenceEquals(binding.Info, expectedInfos[index]))
        {
            Throws.ThrowInputBindingMismatch(index, expectedInfos[index].Name);
        }
    }

    void ValidateOutputBinding(
        OrtValueBinding binding,
        int index,
        ReadOnlySpan<OrtValueBinding> precedingBindings)
    {
        if (!ReferenceEquals(binding.Session, this) || Array.IndexOf(_outputs, binding.Info) < 0)
        {
            Throws.ThrowOutputBindingSessionMismatch(index);
        }
        for (var precedingIndex = 0; precedingIndex < index; ++precedingIndex)
        {
            if (ReferenceEquals(precedingBindings[precedingIndex].Info, binding.Info))
            {
                Throws.ThrowOutputBoundMoreThanOnce(binding.Info.Name);
            }
        }
    }

    void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(IsClosed || IsInvalid, this);

    void ReleaseEnvironmentReference()
    {
        if (!_environmentReferenceAdded)
        {
            return;
        }

        _environment.DangerousRelease();
        _environmentReferenceAdded = false;
    }

    OrtTensorInfo[] GetOverridableInitializerInfos() =>
        GetTensorInfos(TensorInfoKind.OverridableInitializer);

    OrtTensorInfo[] GetTensorInfos(TensorInfoKind kind)
    {
        nuint count;
        Ort.Ok(kind switch
        {
            TensorInfoKind.Input => Ort.SessionGetInputCount((Ort.OrtSession*)handle, &count),
            TensorInfoKind.Output => Ort.SessionGetOutputCount((Ort.OrtSession*)handle, &count),
            _ => Ort.SessionGetOverridableInitializerCount((Ort.OrtSession*)handle, &count),
        });
        var infos = new OrtTensorInfo[checked((int)count)];
        var initializedCount = 0;
        try
        {
            for (nuint index = 0; index < count; ++index)
            {
                infos[checked((int)index)] = GetTensorInfo(index, kind);
                ++initializedCount;
            }
            return infos;
        }
        catch
        {
            for (var index = 0; index < initializedCount; ++index)
            {
                infos[index].Dispose();
            }
            throw;
        }
    }

    OrtTensorInfo GetTensorInfo(nuint index, TensorInfoKind kind)
    {
        Ort.OrtAllocator* allocator;
        Ort.Ok(Ort.GetAllocatorWithDefaultOptions(&allocator));
        sbyte* nativeName;
        Ort.Ok(kind switch
        {
            TensorInfoKind.Input => Ort.SessionGetInputName((Ort.OrtSession*)handle, index, allocator, &nativeName),
            TensorInfoKind.Output => Ort.SessionGetOutputName((Ort.OrtSession*)handle, index, allocator, &nativeName),
            _ => Ort.SessionGetOverridableInitializerName(
                (Ort.OrtSession*)handle,
                index,
                allocator,
                &nativeName),
        });
        try
        {
            var name = Marshal.PtrToStringUTF8((IntPtr)nativeName) ??
                Throws.ThrowNodeNameMissing<string>();
            Ort.OrtTypeInfo* typeInfo;
            Ort.Ok(kind switch
            {
                TensorInfoKind.Input => Ort.SessionGetInputTypeInfo((Ort.OrtSession*)handle, index, &typeInfo),
                TensorInfoKind.Output => Ort.SessionGetOutputTypeInfo((Ort.OrtSession*)handle, index, &typeInfo),
                _ => Ort.SessionGetOverridableInitializerTypeInfo(
                    (Ort.OrtSession*)handle,
                    index,
                    &typeInfo),
            });
            try
            {
                Ort.OrtTensorTypeAndShapeInfo* tensorInfo;
                Ort.Ok(Ort.CastTypeInfoToTensorInfo(typeInfo, &tensorInfo));
                nuint dimensionCount;
                Ort.Ok(Ort.GetDimensionsCount(tensorInfo, &dimensionCount));
                var dimensions = new long[checked((int)dimensionCount)];
                fixed (long* dimensionsPointer = dimensions)
                {
                    Ort.Ok(Ort.GetDimensions(tensorInfo, dimensionsPointer, dimensionCount));
                }
                Ort.ONNXTensorElementDataType elementType;
                Ort.Ok(Ort.GetTensorElementType(tensorInfo, &elementType));
                var symbolicDimensionPointers = stackalloc sbyte*[checked((int)dimensionCount)];
                Ort.Ok(Ort.GetSymbolicDimensions(
                    tensorInfo,
                    symbolicDimensionPointers,
                    dimensionCount));
                var symbolicDimensions = new string?[checked((int)dimensionCount)];
                for (var dimensionIndex = 0; dimensionIndex < symbolicDimensions.Length; ++dimensionIndex)
                {
                    symbolicDimensions[dimensionIndex] = symbolicDimensionPointers[dimensionIndex] is null
                        ? null
                        : Marshal.PtrToStringUTF8((IntPtr)symbolicDimensionPointers[dimensionIndex]);
                }
                return new OrtTensorInfo(
                    allocator,
                    nativeName,
                    name,
                    dimensions,
                    symbolicDimensions,
                    elementType);
            }
            finally
            {
                Ort.ReleaseTypeInfo(typeInfo);
            }
        }
        catch
        {
            Ort.ReleaseAllocatorValue(allocator, nativeName);
            throw;
        }
    }

    OrtModelMetadata GetModelMetadata()
    {
        Ort.OrtAllocator* allocator;
        Ort.Ok(Ort.GetAllocatorWithDefaultOptions(&allocator));
        Ort.OrtModelMetadata* metadata;
        Ort.Ok(Ort.SessionGetModelMetadata((Ort.OrtSession*)handle, &metadata));
        try
        {
            long version;
            Ort.Ok(Ort.ModelMetadataGetVersion(metadata, &version));
            var customMetadata = GetCustomMetadata(metadata, allocator);
            return new OrtModelMetadata(
                GetMetadataString(metadata, allocator, MetadataStringKind.ProducerName),
                GetMetadataString(metadata, allocator, MetadataStringKind.GraphName),
                GetMetadataString(metadata, allocator, MetadataStringKind.GraphDescription),
                GetMetadataString(metadata, allocator, MetadataStringKind.Domain),
                GetMetadataString(metadata, allocator, MetadataStringKind.Description),
                version,
                customMetadata);
        }
        finally
        {
            Ort.ReleaseModelMetadata(metadata);
        }
    }

    static Dictionary<string, string> GetCustomMetadata(
        Ort.OrtModelMetadata* metadata,
        Ort.OrtAllocator* allocator)
    {
        sbyte** keys;
        long keyCount;
        Ort.Ok(Ort.ModelMetadataGetCustomMetadataMapKeys(metadata, allocator, &keys, &keyCount));
        var result = new Dictionary<string, string>(checked((int)keyCount), StringComparer.Ordinal);
        try
        {
            for (var index = 0; index < keyCount; ++index)
            {
                var keyPointer = keys[index];
                var key = Marshal.PtrToStringUTF8((IntPtr)keyPointer) ??
                    Throws.ThrowMetadataKeyMissing<string>();
                sbyte* valuePointer;
                Ort.Ok(Ort.ModelMetadataLookupCustomMetadataMap(
                    metadata,
                    allocator,
                    keyPointer,
                    &valuePointer));
                try
                {
                    result.Add(key, Marshal.PtrToStringUTF8((IntPtr)valuePointer) ?? string.Empty);
                }
                finally
                {
                    Ort.ReleaseAllocatorValue(allocator, valuePointer);
                }
            }
            return result;
        }
        finally
        {
            for (var index = 0; index < keyCount; ++index)
            {
                Ort.ReleaseAllocatorValue(allocator, keys[index]);
            }
            Ort.ReleaseAllocatorValue(allocator, keys);
        }
    }

    static string GetMetadataString(
        Ort.OrtModelMetadata* metadata,
        Ort.OrtAllocator* allocator,
        MetadataStringKind kind)
    {
        sbyte* value;
        Ort.Ok(kind switch
        {
            MetadataStringKind.ProducerName => Ort.ModelMetadataGetProducerName(metadata, allocator, &value),
            MetadataStringKind.GraphName => Ort.ModelMetadataGetGraphName(metadata, allocator, &value),
            MetadataStringKind.GraphDescription => Ort.ModelMetadataGetGraphDescription(metadata, allocator, &value),
            MetadataStringKind.Domain => Ort.ModelMetadataGetDomain(metadata, allocator, &value),
            _ => Ort.ModelMetadataGetDescription(metadata, allocator, &value),
        });
        try
        {
            return Marshal.PtrToStringUTF8((IntPtr)value) ?? string.Empty;
        }
        finally
        {
            Ort.ReleaseAllocatorValue(allocator, value);
        }
    }

    enum TensorInfoKind
    {
        Input,
        Output,
        OverridableInitializer,
    }

    enum MetadataStringKind
    {
        ProducerName,
        GraphName,
        GraphDescription,
        Domain,
        Description,
    }
}
