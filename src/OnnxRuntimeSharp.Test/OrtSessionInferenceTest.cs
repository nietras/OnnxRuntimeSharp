using System;
using System.IO;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtSessionInferenceTest
{
    [TestMethod]
    public void PreallocatedInferenceProducesFiniteScores()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();

        session.Run(input, output);

        foreach (var value in output.Data)
        {
            Assert.IsTrue(float.IsFinite(value));
        }
    }

    [TestMethod]
    public void OrtAllocatedOutputsExposeTypeShapeAndData()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        var inputs = new[] { session.CreateInputBinding(0, input) };

        var outputs = session.Run(inputs);
        try
        {
            Assert.HasCount(1, outputs);
            var output = outputs[0];
            CollectionAssert.AreEqual(new long[] { 1, 10 }, output.Dimensions.ToArray());
            foreach (var value in output.GetTensorData<float>())
            {
                Assert.IsTrue(float.IsFinite(value));
            }
        }
        finally
        {
            foreach (var output in outputs)
            {
                output.Dispose();
            }
        }
    }

    [TestMethod]
    public void RunOptionsCanBeUsedForInference()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();
        using var runOptions = new OrtRunOptions { Tag = "test" };

        session.Run(input, output, runOptions);
    }

    [TestMethod]
    public void WrongElementTypeBindingIsRejected()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = new OrtTensor<byte>(new byte[28 * 28], [1, 1, 28, 28]);

        Assert.ThrowsExactly<ArgumentException>(() => session.CreateInputBinding(0, input));
    }

    [TestMethod]
    public void SingleValueOverloadRejectsMultiValueAssumptionsThroughMetadata()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);

        Assert.HasCount(1, session.Inputs);
        Assert.HasCount(1, session.Outputs);
    }

    [TestMethod]
    public void ProfilingReturnsTracePath()
    {
        var prefix = Path.Combine(Path.GetTempPath(), $"ort-{Guid.NewGuid():N}");
        using var environment = new OrtEnvironment();
        using var options = new OrtSessionOptions();
        options.EnableProfiling(prefix);
        using var session = TestData.CreateMnistSession(environment, options);
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();
        session.Run(input, output);

        var tracePath = session.EndProfiling();
        try
        {
            Assert.IsTrue(File.Exists(tracePath));
        }
        finally
        {
            File.Delete(tracePath);
        }
    }

    [TestMethod]
    public void OrtExceptionIncludesNativeErrorCode()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = new OrtTensor<float>(new float[28 * 28], [1, 1, 28, 28]);
        using var output = new OrtTensor<float>(new float[9], [1, 9]);

        var exception = Assert.ThrowsExactly<OrtException>(() => session.Run(input, output));

        Assert.AreNotEqual(Ort.OrtErrorCode.ORT_OK, exception.ErrorCode);
    }
}
