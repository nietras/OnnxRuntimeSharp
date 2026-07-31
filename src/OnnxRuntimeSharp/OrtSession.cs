using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtSession : SafeHandle
{
    readonly OrtEnvironment _environment;
    readonly OrtTensorInfo[] _inputs = [];
    readonly OrtTensorInfo[] _outputs = [];

    public OrtSession(OrtEnvironment environment, ReadOnlySpan<byte> model, OrtSessionOptions? options = null)
        : base(IntPtr.Zero, ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(environment);
        if (model.IsEmpty)
        {
            throw new ArgumentException("Model data cannot be empty.", nameof(model));
        }

        _environment = environment;
        var ownsOptions = options is null;
        options ??= new OrtSessionOptions();
        try
        {
            fixed (byte* modelPointer = model)
            {
                Ort.OrtSession* session;
                Ort.ThrowIfError(Ort.CreateSessionFromArray(
                    (Ort.OrtEnv*)environment.DangerousGetHandle(),
                    modelPointer,
                    (nuint)model.Length,
                    (Ort.OrtSessionOptions*)options.DangerousGetHandle(),
                    &session));
                SetHandle((IntPtr)session);
            }

            _inputs = GetTensorInfos(isInput: true);
            _outputs = GetTensorInfos(isInput: false);
        }
        catch
        {
            Dispose();
            throw;
        }
        finally
        {
            if (ownsOptions)
            {
                options.Dispose();
            }
        }
    }

    public IReadOnlyList<OrtTensorInfo> Inputs => _inputs;

    public IReadOnlyList<OrtTensorInfo> Outputs => _outputs;

    public string InputName => _inputs[0].Name;

    public string OutputName => _outputs[0].Name;

    public ReadOnlyMemory<long> InputDimensions => _inputs[0].Dimensions;

    public ReadOnlyMemory<long> OutputDimensions => _outputs[0].Dimensions;

    public OrtValueBinding CreateInputBinding<T>(int index, OrtTensor<T> value)
        where T : unmanaged =>
        CreateBinding(_inputs, index, value, nameof(index));

    public OrtValueBinding CreateOutputBinding<T>(int index, OrtTensor<T> value)
        where T : unmanaged =>
        CreateBinding(_outputs, index, value, nameof(index));

    public void Run<TInput, TOutput>(OrtTensor<TInput> input, OrtTensor<TOutput> output)
        where TInput : unmanaged
        where TOutput : unmanaged
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        var inputName = _inputs[0].NamePointer;
        var outputName = _outputs[0].NamePointer;
        var inputValue = (Ort.OrtValue*)input.DangerousGetHandle();
        var outputValue = (Ort.OrtValue*)output.DangerousGetHandle();
        Ort.ThrowIfError(Ort.Run(
            (Ort.OrtSession*)handle,
            null,
            &inputName,
            &inputValue,
            1,
            &outputName,
            1,
            &outputValue));
    }

    public void Run(ReadOnlySpan<OrtValueBinding> inputs, ReadOnlySpan<OrtValueBinding> outputs)
    {
        if (inputs.Length != _inputs.Length)
        {
            throw new ArgumentException($"Expected {_inputs.Length} input bindings, got {inputs.Length}.", nameof(inputs));
        }
        if (outputs.Length == 0)
        {
            throw new ArgumentException("At least one output binding is required.", nameof(outputs));
        }

        var inputNames = stackalloc sbyte*[inputs.Length];
        var inputValues = stackalloc Ort.OrtValue*[inputs.Length];
        var outputNames = stackalloc sbyte*[outputs.Length];
        var outputValues = stackalloc Ort.OrtValue*[outputs.Length];
        for (var index = 0; index < inputs.Length; ++index)
        {
            inputNames[index] = inputs[index].NamePointer;
            inputValues[index] = inputs[index].ValuePointer;
        }
        for (var index = 0; index < outputs.Length; ++index)
        {
            outputNames[index] = outputs[index].NamePointer;
            outputValues[index] = outputs[index].ValuePointer;
        }

        Ort.ThrowIfError(Ort.Run(
            (Ort.OrtSession*)handle,
            null,
            inputNames,
            inputValues,
            (nuint)inputs.Length,
            outputNames,
            (nuint)outputs.Length,
            outputValues));
    }

    public string EndProfiling()
    {
        Ort.OrtAllocator* allocator;
        Ort.ThrowIfError(Ort.GetAllocatorWithDefaultOptions(&allocator));
        sbyte* profilePath;
        Ort.ThrowIfError(Ort.SessionEndProfiling((Ort.OrtSession*)handle, allocator, &profilePath));
        try
        {
            return Marshal.PtrToStringUTF8((IntPtr)profilePath) ?? string.Empty;
        }
        finally
        {
            Ort.ThrowIfError(Ort.AllocatorFree(allocator, profilePath));
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
        Ort.ReleaseSession((Ort.OrtSession*)handle);
        return true;
    }

    OrtValueBinding CreateBinding<T>(OrtTensorInfo[] infos, int index, OrtTensor<T> value, string parameterName)
        where T : unmanaged
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, infos.Length);
        ArgumentNullException.ThrowIfNull(value);
        if (infos[index].ElementType != value.ElementType)
        {
            throw new ArgumentException(
                $"Tensor '{infos[index].Name}' expects {infos[index].ElementType}, but received {value.ElementType}.",
                parameterName);
        }

        return new OrtValueBinding(infos[index], value);
    }

    OrtTensorInfo[] GetTensorInfos(bool isInput)
    {
        nuint count;
        Ort.ThrowIfError(isInput
            ? Ort.SessionGetInputCount((Ort.OrtSession*)handle, &count)
            : Ort.SessionGetOutputCount((Ort.OrtSession*)handle, &count));
        var infos = new OrtTensorInfo[checked((int)count)];
        var initializedCount = 0;
        try
        {
            for (nuint index = 0; index < count; ++index)
            {
                infos[checked((int)index)] = GetTensorInfo(index, isInput);
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

    OrtTensorInfo GetTensorInfo(nuint index, bool isInput)
    {
        Ort.OrtAllocator* allocator;
        Ort.ThrowIfError(Ort.GetAllocatorWithDefaultOptions(&allocator));
        sbyte* nativeName;
        Ort.ThrowIfError(isInput
            ? Ort.SessionGetInputName((Ort.OrtSession*)handle, index, allocator, &nativeName)
            : Ort.SessionGetOutputName((Ort.OrtSession*)handle, index, allocator, &nativeName));
        try
        {
            var name = Marshal.PtrToStringUTF8((IntPtr)nativeName) ??
                throw new InvalidOperationException("ONNX Runtime returned a null node name.");
            Ort.OrtTypeInfo* typeInfo;
            Ort.ThrowIfError(isInput
                ? Ort.SessionGetInputTypeInfo((Ort.OrtSession*)handle, index, &typeInfo)
                : Ort.SessionGetOutputTypeInfo((Ort.OrtSession*)handle, index, &typeInfo));
            try
            {
                Ort.OrtTensorTypeAndShapeInfo* tensorInfo;
                Ort.ThrowIfError(Ort.CastTypeInfoToTensorInfo(typeInfo, &tensorInfo));
                nuint dimensionCount;
                Ort.ThrowIfError(Ort.GetDimensionsCount(tensorInfo, &dimensionCount));
                var dimensions = new long[checked((int)dimensionCount)];
                fixed (long* dimensionsPointer = dimensions)
                {
                    Ort.ThrowIfError(Ort.GetDimensions(tensorInfo, dimensionsPointer, dimensionCount));
                }
                Ort.ONNXTensorElementDataType elementType;
                Ort.ThrowIfError(Ort.GetTensorElementType(tensorInfo, &elementType));
                return new OrtTensorInfo(allocator, nativeName, name, dimensions, elementType);
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
}
