using System;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtTensorTest
{
    [TestMethod]
    public void ManagedTensorExposesPinnedData()
    {
        var data = new float[6];
        using var tensor = new OrtTensor<float>(data, [1, 2, 3]);

        tensor.Data[1] = 42;

        Assert.AreEqual(42f, data[1]);
        Assert.AreEqual(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT, tensor.ElementType);
    }

    [TestMethod]
    [DataRow(1, 2)]
    [DataRow(2, 1)]
    public void MismatchedDimensionsAreRejected(int firstDimension, int secondDimension)
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            new OrtTensor<float>(new float[3], [firstDimension, secondDimension]));
    }

    [TestMethod]
    public void NegativeDimensionsAreRejected()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new OrtTensor<float>(new float[1], [-1]));
    }

    [TestMethod]
    public void UnsignedIntegerTypesAreSupported()
    {
        using var uintTensor = new OrtTensor<uint>(new uint[1], [1]);
        using var ulongTensor = new OrtTensor<ulong>(new ulong[1], [1]);

        Assert.AreEqual(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT32, uintTensor.ElementType);
        Assert.AreEqual(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT64, ulongTensor.ElementType);
    }

    [TestMethod]
    public unsafe void NativeMemoryTensorRetainsMemoryInfo()
    {
        using var memoryInfo = OrtMemoryInfo.CreateCpu();
        var data = stackalloc float[4];
        using var tensor = new OrtTensor<float>(data, 4, [1, 4], memoryInfo);

        Assert.AreEqual(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT, tensor.ElementType);
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = tensor.Data);
    }

    [TestMethod]
    public void DisposedTensorRejectsManagedDataAccess()
    {
        var tensor = new OrtTensor<float>(new float[1], [1]);
        tensor.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => _ = tensor.Data);
    }

    [TestMethod]
    public void ScalarTensorIsSupported()
    {
        using var tensor = new OrtTensor<float>(new float[1], []);

        Assert.HasCount(1, tensor.Data);
    }

    [TestMethod]
    public void ManagedTensorArgumentsAreValidated()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new OrtTensor<float>(null!, [1]));
        Assert.ThrowsExactly<ArgumentException>(() =>
            new OrtTensor<float>([], [0]));
        Assert.ThrowsExactly<OverflowException>(() =>
            new OrtTensor<float>(new float[1], [long.MaxValue, 2]));
    }

    [TestMethod]
    public unsafe void NativeTensorArgumentsAreValidated()
    {
        using var memoryInfo = OrtMemoryInfo.CreateCpu();
        var data = stackalloc float[4];

        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new OrtTensor<float>(null, 4, [4], memoryInfo));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new OrtTensor<float>(data, 0, [0], memoryInfo));
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new OrtTensor<float>(data, 4, [4], null!));
        Assert.ThrowsExactly<ArgumentException>(() =>
            new OrtTensor<float>(data, 4, [5], memoryInfo));
    }

    [TestMethod]
    public void AllSupportedElementTypesAreMapped()
    {
        AssertElementType<float>(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT);
        AssertElementType<byte>(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT8);
        AssertElementType<sbyte>(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_INT8);
        AssertElementType<ushort>(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT16);
        AssertElementType<short>(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_INT16);
        AssertElementType<int>(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_INT32);
        AssertElementType<long>(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_INT64);
        AssertElementType<bool>(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_BOOL);
        AssertElementType<Half>(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT16);
        AssertElementType<double>(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_DOUBLE);
        AssertElementType<uint>(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT32);
        AssertElementType<ulong>(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT64);
        Assert.ThrowsExactly<NotSupportedException>(() => OrtTensorElementType.Get<DummyValue>());
    }

    static void AssertElementType<T>(Ort.ONNXTensorElementDataType expected) where T : unmanaged =>
        Assert.AreEqual(expected, OrtTensorElementType.Get<T>());

    readonly struct DummyValue;
}
