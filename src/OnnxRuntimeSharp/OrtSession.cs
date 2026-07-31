using System;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtSession : SafeHandle
{
    readonly OrtEnvironment _environment;
    readonly nint _inputName;
    readonly nint _outputName;
    readonly long[] _inputDimensions;
    readonly long[] _outputDimensions;

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

            _inputName = GetInputName();
            _outputName = GetOutputName();
            _inputDimensions = GetTensorDimensions(isInput: true);
            _outputDimensions = GetTensorDimensions(isInput: false);
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

    public string InputName => Marshal.PtrToStringUTF8(_inputName)!;

    public string OutputName => Marshal.PtrToStringUTF8(_outputName)!;

    public ReadOnlyMemory<long> InputDimensions => _inputDimensions;

    public ReadOnlyMemory<long> OutputDimensions => _outputDimensions;

    public void Run<TInput, TOutput>(OrtTensor<TInput> input, OrtTensor<TOutput> output)
        where TInput : unmanaged
        where TOutput : unmanaged
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        var inputName = _inputName;
        var outputName = _outputName;
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
        Marshal.FreeCoTaskMem(_inputName);
        Marshal.FreeCoTaskMem(_outputName);
        Ort.ReleaseSession((Ort.OrtSession*)handle);
        return true;
    }

    nint GetInputName()
    {
        Ort.OrtAllocator* allocator;
        Ort.ThrowIfError(Ort.GetAllocatorWithDefaultOptions(&allocator));
        sbyte* nativeName;
        Ort.ThrowIfError(Ort.SessionGetInputName((Ort.OrtSession*)handle, 0, allocator, &nativeName));
        try
        {
            return Marshal.StringToCoTaskMemUTF8(Marshal.PtrToStringUTF8((nint)nativeName));
        }
        finally
        {
            Ort.ThrowIfError(Ort.AllocatorFree(allocator, nativeName));
        }
    }

    nint GetOutputName()
    {
        Ort.OrtAllocator* allocator;
        Ort.ThrowIfError(Ort.GetAllocatorWithDefaultOptions(&allocator));
        sbyte* nativeName;
        Ort.ThrowIfError(Ort.SessionGetOutputName((Ort.OrtSession*)handle, 0, allocator, &nativeName));
        try
        {
            return Marshal.StringToCoTaskMemUTF8(Marshal.PtrToStringUTF8((nint)nativeName));
        }
        finally
        {
            Ort.ThrowIfError(Ort.AllocatorFree(allocator, nativeName));
        }
    }

    long[] GetTensorDimensions(bool isInput)
    {
        Ort.OrtTypeInfo* typeInfo;
        Ort.ThrowIfError(isInput
            ? Ort.SessionGetInputTypeInfo((Ort.OrtSession*)handle, 0, &typeInfo)
            : Ort.SessionGetOutputTypeInfo((Ort.OrtSession*)handle, 0, &typeInfo));
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
            return dimensions;
        }
        finally
        {
            Ort.ReleaseTypeInfo(typeInfo);
        }
    }
}
