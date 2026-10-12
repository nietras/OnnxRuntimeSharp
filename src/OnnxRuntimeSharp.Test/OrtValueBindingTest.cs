using System;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtValueBindingTest
{
    [TestMethod]
    public void BindingExposesModelValueMetadata()
    {
        using var env = new OrtEnv();
        using var session = TestData.CreateMnistSession(env);
        using var input = TestData.CreateMnistInput();

        var binding = session.CreateInputBinding(0, input);

        Assert.AreSame(session.Inputs.Span[0], binding.Info);
        Assert.AreSame(input, binding.Value);
        Assert.AreEqual(input.Handle, binding.ValueHandle);
    }

    [TestMethod]
    public void BindingFromAnotherSessionIsRejected()
    {
        using var env = new OrtEnv();
        using var firstSession = TestData.CreateMnistSession(env);
        using var secondSession = TestData.CreateMnistSession(env);
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();
        var inputs = new[] { firstSession.CreateInputBinding(0, input) };
        var outputs = new[] { secondSession.CreateOutputBinding(0, output) };

        Assert.ThrowsExactly<ArgumentException>(() => secondSession.Run(inputs, outputs));
    }

    [TestMethod]
    public void NativeOutputsCanBeReusedThroughCommonValueBindings()
    {
        using var env = new OrtEnv();
        using var session = TestData.CreateTwoInputSession(env);
        using var firstInput = new OrtValue<float>([3], [1]);
        using var secondInput = new OrtValue<float>([7], [1]);
        var results = session.Run([
            session.CreateInputBinding(0, firstInput),
            session.CreateInputBinding(1, secondInput),
        ]);
        using var firstResult = results[0];
        using var secondResult = results[1];
        using var firstOutput = new OrtValue<float>(new float[1], [1]);
        using var secondOutput = new OrtValue<float>(new float[1], [1]);

        session.Run([
            session.CreateInputBinding(0, firstResult),
            session.CreateInputBinding(1, secondResult),
        ], [
            session.CreateOutputBinding(0, firstOutput),
            session.CreateOutputBinding(1, secondOutput),
        ]);

        Assert.AreEqual(3f, firstOutput.Data[0]);
        Assert.AreEqual(7f, secondOutput.Data[0]);
    }

    [TestMethod]
    public void DefaultBindingIsRejected()
    {
        using var env = new OrtEnv();
        using var session = TestData.CreateMnistSession(env);
        using var output = TestData.CreateMnistOutput();

        Assert.ThrowsExactly<ArgumentException>(() =>
            session.Run([default], [session.CreateOutputBinding(0, output)]));
    }
}
