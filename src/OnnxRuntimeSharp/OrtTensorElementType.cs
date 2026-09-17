using System;

namespace OnnxRuntimeSharp;

static class OrtTensorElementType
{
    public static Ort.ONNXTensorElementDataType Get<T>() where T : unmanaged =>
        typeof(T) == typeof(float) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT
        : typeof(T) == typeof(byte) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT8
        : typeof(T) == typeof(sbyte) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_INT8
        : typeof(T) == typeof(ushort) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT16
        : typeof(T) == typeof(short) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_INT16
        : typeof(T) == typeof(int) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_INT32
        : typeof(T) == typeof(long) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_INT64
        : typeof(T) == typeof(bool) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_BOOL
        : typeof(T) == typeof(Half) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT16
        : typeof(T) == typeof(double) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_DOUBLE
        : typeof(T) == typeof(uint) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT32
        : typeof(T) == typeof(ulong) ? Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT64
        : Throws.ThrowTensorInteropNotSupported<Ort.ONNXTensorElementDataType>(typeof(T));
}
