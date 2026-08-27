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
        Assert.IsNotNull(session.ModelMetadata.GraphName);
        Assert.IsNotNull(session.ModelMetadata.GraphDescription);
        Assert.IsNotNull(session.ModelMetadata.Domain);
        Assert.IsNotNull(session.ModelMetadata.Description);
        _ = session.ModelMetadata.Version;
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

    [TestMethod]
    public void TensorInfoDisposalIsIdempotent()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        var info = session.Inputs[0];

        info.Dispose();
        info.Dispose();
    }

    [TestMethod]
    public void GeneratedModelMetadataAndMultipleValuesAreAvailable()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateTwoInputSession(environment);

        Assert.HasCount(2, session.Inputs);
        Assert.HasCount(2, session.Outputs);
        Assert.AreEqual("OnnxRuntimeSharp.Test", session.ModelMetadata.ProducerName);
        Assert.AreEqual("TwoInputTwoOutput", session.ModelMetadata.GraphName);
        Assert.AreEqual("test", session.ModelMetadata.Domain);
        Assert.AreEqual("Two independent identity operations.", session.ModelMetadata.Description);
        Assert.AreEqual(1, session.ModelMetadata.Version);
        Assert.AreEqual("coverage", session.ModelMetadata.CustomMetadata["purpose"]);
    }
}
