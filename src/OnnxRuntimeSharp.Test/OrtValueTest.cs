using System;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtValueTest
{
    [TestMethod]
    public unsafe void NullNativeValueIsRejected()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new OrtValue(null));
    }

    [TestMethod]
    public void OrtAllocatedTensorExposesTypedData()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        var inputs = new[] { session.CreateInputBinding(0, input) };
        var outputs = session.Run(inputs);
        Assert.HasCount(1, outputs);
        using var output = outputs[0];

        Assert.AreEqual(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT, output.ElementType);
        CollectionAssert.AreEqual(new long[] { 1, 10 }, output.Dimensions.ToArray());
        Assert.AreEqual(10, output.GetTensorData<float>().Length);
        Assert.ThrowsExactly<InvalidOperationException>(() => { _ = output.GetTensorData<int>().Length; });
    }

    [TestMethod]
    public void DisposedValueRejectsDataAccess()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        var outputs = session.Run([session.CreateInputBinding(0, input)]);
        Assert.HasCount(1, outputs);
        var output = outputs[0];
        output.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => { _ = output.GetTensorData<float>().Length; });
    }
}
