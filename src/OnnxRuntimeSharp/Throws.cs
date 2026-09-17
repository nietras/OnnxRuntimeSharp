using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace OnnxRuntimeSharp;

static class Throws
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowApiBaseUnavailable()
    {
        throw new InvalidOperationException("ONNX Runtime did not return an API base.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowApiVersionUnavailable(uint apiVersion)
    {
        throw new NotSupportedException($"ONNX Runtime C API version {apiVersion} is unavailable.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowExecutionProviderNameMissing<T>()
    {
        throw new InvalidOperationException("ONNX Runtime returned a null execution provider name.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowModelDataEmpty()
    {
        throw new ArgumentException("Model data cannot be empty.", "model");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowIoBindingSessionMismatch()
    {
        throw new ArgumentException("The I/O binding belongs to a different session.", "binding");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowSingleInputOutputModelRequired()
    {
        throw new InvalidOperationException(
            "This overload requires a model with exactly one input and one output. Use value bindings instead.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInputBindingCountMismatch(int expectedCount, int actualCount)
    {
        throw new ArgumentException($"Expected {expectedCount} input bindings, got {actualCount}.", "inputs");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowOutputBindingCountMismatch(int maximumCount, int actualCount)
    {
        throw new ArgumentException(
            $"Expected between 1 and {maximumCount} output bindings, got {actualCount}.",
            "outputs");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowBindingValueDisposed(string objectName)
    {
        throw new ObjectDisposedException(objectName);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowTensorBindingTypeMismatch(
        string tensorName,
        Ort.ONNXTensorElementDataType expectedType,
        Ort.ONNXTensorElementDataType actualType)
    {
        throw new ArgumentException(
            $"Tensor '{tensorName}' expects {expectedType}, but received {actualType}.",
            "index");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInputBindingMismatch(int index, string valueName)
    {
        throw new ArgumentException(
            $"Binding at index {index} does not match this session's '{valueName}' value.",
            "inputs");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowOutputBindingSessionMismatch(int index)
    {
        throw new ArgumentException(
            $"Output binding at index {index} does not belong to this session.",
            "outputs");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowOutputBoundMoreThanOnce(string outputName)
    {
        throw new ArgumentException($"Output '{outputName}' is bound more than once.", "outputs");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowNodeNameMissing<T>()
    {
        throw new InvalidOperationException("ONNX Runtime returned a null node name.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowMetadataKeyMissing<T>()
    {
        throw new InvalidOperationException("ONNX Runtime returned a null metadata key.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNativeValueNull()
    {
        throw new ArgumentNullException("value");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowTensorElementTypeMismatch(
        Ort.ONNXTensorElementDataType actualType,
        Ort.ONNXTensorElementDataType expectedType)
    {
        throw new InvalidOperationException($"Tensor contains {actualType}, not {expectedType}.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowTensorDataEmpty()
    {
        throw new ArgumentException("Tensor data cannot be empty.", "data");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowTensorDimensionsDataLengthMismatch()
    {
        throw new ArgumentException("Tensor dimensions do not match data length.", "dimensions");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNativeTensorDataNull()
    {
        throw new ArgumentNullException("data");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowTensorDimensionsElementCountMismatch()
    {
        throw new ArgumentException("Tensor dimensions do not match element count.", "dimensions");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowExternallyOwnedTensorData()
    {
        throw new InvalidOperationException("The tensor wraps externally owned native memory.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNegativeTensorDimension()
    {
        throw new ArgumentOutOfRangeException("dimensions", "Tensor dimensions must be non-negative.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowTensorInteropNotSupported<T>(Type elementType)
    {
        throw new NotSupportedException($"ONNX Runtime does not support {elementType} tensor interop.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowExecutionProviderDevicesEmpty()
    {
        throw new ArgumentException("At least one execution-provider device is required.", "devices");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowExecutionProviderDeviceEnvironmentMismatch()
    {
        throw new ArgumentException("All devices must originate from the supplied environment.", "devices");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowExecutionProviderDeviceNameMismatch()
    {
        throw new ArgumentException("All devices must belong to the same execution provider.", "devices");
    }
}
