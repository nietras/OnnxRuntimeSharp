using System;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtSessionMetadataTest
{
    [TestMethod]
    public void MnistMetadataIsAvailable()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);

        Assert.HasCount(1, session.Inputs);
        Assert.HasCount(1, session.Outputs);
        var input = session.Inputs[0];
        var output = session.Outputs[0];
        Assert.AreEqual("Input3", input.Name);
        CollectionAssert.AreEqual(new long[] { 1, 1, 28, 28 }, input.Dimensions.ToArray());
        Assert.AreEqual(4, input.SymbolicDimensions.Length);
        Assert.AreEqual(Ort.ONNXTensorElementDataType.ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT, input.ElementType);
        CollectionAssert.AreEqual(new long[] { 1, 10 }, output.Dimensions.ToArray());
        Assert.IsEmpty(session.OverridableInitializers);
        Assert.IsNotNull(session.ModelMetadata.ProducerName);
        Assert.IsNotNull(session.ModelMetadata.CustomMetadata);
    }

    [TestMethod]
    public void SessionCanLoadModelDirectlyFromPath()
    {
        using var environment = new OrtEnvironment();
        using var session = new OrtSession(environment, TestData.MnistModelPath);

        Assert.HasCount(1, session.Inputs);
        Assert.HasCount(1, session.Outputs);
    }
}
