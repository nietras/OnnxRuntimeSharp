using System;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtValueBindingTest
{
    [TestMethod]
    public void BindingExposesModelValueMetadata()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();

        var binding = session.CreateInputBinding(0, input);

        Assert.AreSame(session.Inputs[0], binding.Info);
    }

    [TestMethod]
    public void BindingFromAnotherSessionIsRejected()
    {
        using var environment = new OrtEnvironment();
        using var firstSession = TestData.CreateMnistSession(environment);
        using var secondSession = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();
        var inputs = new[] { firstSession.CreateInputBinding(0, input) };
        var outputs = new[] { secondSession.CreateOutputBinding(0, output) };

        Assert.ThrowsExactly<ArgumentException>(() => secondSession.Run(inputs, outputs));
    }

    [TestMethod]
    public void DefaultBindingIsRejected()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var output = TestData.CreateMnistOutput();

        Assert.ThrowsExactly<ArgumentException>(() =>
            session.Run([default], [session.CreateOutputBinding(0, output)]));
    }
}
