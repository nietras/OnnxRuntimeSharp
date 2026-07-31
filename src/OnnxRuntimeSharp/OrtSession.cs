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
        : base(nint.Zero, ownsHandle: true)
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
                SetHandle((nint)session);
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
        var inputName = _inputs[0].NameHandle;
        var outputName = _outputs[0].NameHandle;
        nint inputValue = input.DangerousGetHandle();
        nint outputValue = output.DangerousGetHandle();
        Ort.ThrowIfError(Ort.Run(
            (Ort.OrtSession*)handle,
            null,
            (sbyte**)&inputName,
            (Ort.OrtValue**)&inputValue,
            1,
            (sbyte**)&outputName,
            1,
            (Ort.OrtValue**)&outputValue));
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

        Span<nint> inputNames = stackalloc nint[inputs.Length];
        Span<nint> inputValues = stackalloc nint[inputs.Length];
        Span<nint> outputNames = stackalloc nint[outputs.Length];
        Span<nint> outputValues = stackalloc nint[outputs.Length];
        for (var index = 0; index < inputs.Length; ++index)
        {
            inputNames[index] = inputs[index].NameHandle;
            inputValues[index] = inputs[index].ValueHandle;
        }
        for (var index = 0; index < outputs.Length; ++index)
        {
            outputNames[index] = outputs[index].NameHandle;
            outputValues[index] = outputs[index].ValueHandle;
        }

        fixed (nint* inputNamesPointer = inputNames)
        fixed (nint* inputValuesPointer = inputValues)
        fixed (nint* outputNamesPointer = outputNames)
        fixed (nint* outputValuesPointer = outputValues)
        {
            Ort.ThrowIfError(Ort.Run(
                (Ort.OrtSession*)handle,
                null,
                (sbyte**)inputNamesPointer,
                (Ort.OrtValue**)inputValuesPointer,
                (nuint)inputs.Length,
                (sbyte**)outputNamesPointer,
                (nuint)outputs.Length,
                (Ort.OrtValue**)outputValuesPointer));
        }
    }

    public string EndProfiling()
    {
        Ort.OrtAllocator* allocator;
        Ort.ThrowIfError(Ort.GetAllocatorWithDefaultOptions(&allocator));
        sbyte* profilePath;
        Ort.ThrowIfError(Ort.SessionEndProfiling((Ort.OrtSession*)handle, allocator, &profilePath));
        try
        {
            return Marshal.PtrToStringUTF8((nint)profilePath) ?? string.Empty;
        }
        finally
        {
            Ort.ThrowIfError(Ort.AllocatorFree(allocator, profilePath));
        }
    }

    public override bool IsInvalid => handle == nint.Zero;

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
        for (nuint index = 0; index < count; ++index)
        {
            infos[checked((int)index)] = GetTensorInfo(index, isInput);
        }
        return infos;
    }

    OrtTensorInfo GetTensorInfo(nuint index, bool isInput)
    {
        Ort.OrtAllocator* allocator;
        Ort.ThrowIfError(Ort.GetAllocatorWithDefaultOptions(&allocator));
        sbyte* nativeName;
        Ort.ThrowIfError(isInput
            ? Ort.SessionGetInputName((Ort.OrtSession*)handle, index, allocator, &nativeName)
            : Ort.SessionGetOutputName((Ort.OrtSession*)handle, index, allocator, &nativeName));
        string name;
        nint nameHandle;
        try
        {
            name = Marshal.PtrToStringUTF8((nint)nativeName) ??
                throw new InvalidOperationException("ONNX Runtime returned a null node name.");
            nameHandle = Marshal.StringToCoTaskMemUTF8(name);
        }
        finally
        {
            Ort.ThrowIfError(Ort.AllocatorFree(allocator, nativeName));
        }

        try
        {
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
                return new OrtTensorInfo(nameHandle, name, dimensions, elementType);
            }
            finally
            {
                Ort.ReleaseTypeInfo(typeInfo);
            }
        }
        catch
        {
            Marshal.FreeCoTaskMem(nameHandle);
            throw;
        }
    }
}
