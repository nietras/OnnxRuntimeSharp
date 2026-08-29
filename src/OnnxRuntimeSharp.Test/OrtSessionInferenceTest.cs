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
        Assert.IsFalse(string.IsNullOrWhiteSpace(exception.Message));
    }

    [TestMethod]
    public void SessionConstructorArgumentsAreValidated()
    {
        var model = TestData.ReadMnistModel();
        using var environment = new OrtEnvironment();

        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new OrtSession(null!, model));
        Assert.ThrowsExactly<ArgumentException>(() =>
            new OrtSession(environment, ReadOnlySpan<byte>.Empty));
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new OrtSession(null!, TestData.MnistModelPath));
        Assert.ThrowsExactly<ArgumentException>(() =>
            new OrtSession(environment, ""));
    }

    [TestMethod]
    public void InvalidModelsReturnStructuredErrorsWithoutLeaks()
    {
        using var environment = new OrtEnvironment();

        var memoryException = Assert.ThrowsExactly<OrtException>(() =>
            new OrtSession(environment, new byte[] { 1, 2, 3, 4 }));
        var pathException = Assert.ThrowsExactly<OrtException>(() =>
            new OrtSession(environment, Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.onnx")));

        Assert.AreNotEqual(Ort.OrtErrorCode.ORT_OK, memoryException.ErrorCode);
        Assert.AreNotEqual(Ort.OrtErrorCode.ORT_OK, pathException.ErrorCode);
    }

    [TestMethod]
    public void SingleValueConvenienceMetadataMatchesCollections()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);

        Assert.AreEqual(session.Inputs[0].Name, session.InputName);
        Assert.AreEqual(session.Outputs[0].Name, session.OutputName);
        CollectionAssert.AreEqual(session.Inputs[0].Dimensions.ToArray(), session.InputDimensions.ToArray());
        CollectionAssert.AreEqual(session.Outputs[0].Dimensions.ToArray(), session.OutputDimensions.ToArray());
    }

    [TestMethod]
    public void BindingIndexesAndCountsAreValidated()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();
        var inputBinding = session.CreateInputBinding(0, input);
        var outputBinding = session.CreateOutputBinding(0, output);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => session.CreateInputBinding(-1, input));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => session.CreateInputBinding(1, input));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => session.CreateOutputBinding(-1, output));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => session.CreateOutputBinding(1, output));
        Assert.ThrowsExactly<ArgumentException>(() => session.Run([], [outputBinding]));
        Assert.ThrowsExactly<ArgumentException>(() => session.Run([inputBinding], []));
        Assert.ThrowsExactly<ArgumentException>(() =>
            session.Run([inputBinding], [outputBinding, outputBinding]));
        Assert.ThrowsExactly<ArgumentException>(() => session.Run([]));
    }

    [TestMethod]
    public void DisposedBoundTensorIsRejected()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();
        var inputs = new[] { session.CreateInputBinding(0, input) };
        var outputs = new[] { session.CreateOutputBinding(0, output) };
        input.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => session.Run(inputs, outputs));
    }

    [TestMethod]
    public void DisposedSessionRejectsBindingAndProfilingOperations()
    {
        using var environment = new OrtEnvironment();
        var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        session.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => session.CreateInputBinding(0, input));
        Assert.ThrowsExactly<ObjectDisposedException>(() => session.CreateIoBinding());
        Assert.ThrowsExactly<ObjectDisposedException>(() => session.EndProfiling());
    }

    [TestMethod]
    public void MultiInputOutputModelRunsWithBindings()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateTwoInputSession(environment);
        using var firstInput = new OrtTensor<float>([3], [1]);
        using var secondInput = new OrtTensor<float>([7], [1]);
        using var firstOutput = new OrtTensor<float>(new float[1], [1]);
        using var secondOutput = new OrtTensor<float>(new float[1], [1]);
        var inputs = new[]
        {
            session.CreateInputBinding(0, firstInput),
            session.CreateInputBinding(1, secondInput),
        };
        var outputs = new[]
        {
            session.CreateOutputBinding(0, firstOutput),
            session.CreateOutputBinding(1, secondOutput),
        };

        session.Run(inputs, outputs);

        Assert.AreEqual(3f, firstOutput.Data[0]);
        Assert.AreEqual(7f, secondOutput.Data[0]);
        Assert.ThrowsExactly<InvalidOperationException>(() => session.Run(firstInput, firstOutput));
        Assert.ThrowsExactly<ArgumentException>(() =>
            session.Run(inputs, [outputs[0], outputs[0]]));
    }
}
