using System;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtIoBindingTest
{
    [TestMethod]
    public void PreallocatedBindingRunsInference()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();
        using var binding = session.CreateIoBinding();
        binding.BindInput(0, input);
        binding.BindOutput(0, output);

        session.Run(binding);
        binding.SynchronizeOutputs();

        foreach (var value in output.Data)
        {
            Assert.IsTrue(float.IsFinite(value));
        }
    }

    [TestMethod]
    public void DeviceTargetedOutputCanBeRetrieved()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        using var memoryInfo = OrtMemoryInfo.CreateCpu();
        using var binding = session.CreateIoBinding();
        binding.BindInput(0, input);
        binding.BindOutputToDevice(0, memoryInfo);

        session.Run(binding);
        var outputs = binding.GetOutputValues();
        try
        {
            Assert.HasCount(1, outputs);
            Assert.AreEqual(10, outputs[0].GetTensorData<float>().Length);
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
    public void BindingsCanBeClearedAndReused()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var binding = session.CreateIoBinding();

        binding.ClearInputs();
        binding.ClearOutputs();
    }

    [TestMethod]
    public void PopulatedBindingsCanBeClearedAndRebound()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();
        using var binding = session.CreateIoBinding();

        binding.BindInput(0, input);
        binding.BindOutput(0, output);
        binding.SynchronizeInputs();
        binding.ClearInputs();
        binding.ClearOutputs();
        binding.BindInput(0, input);
        binding.BindOutput(0, output);
        using var runOptions = new OrtRunOptions();
        session.Run(binding, runOptions);
    }

    [TestMethod]
    public void OrtValueOutputCanBeBound()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        var values = session.Run([session.CreateInputBinding(0, input)]);
        using var output = values[0];
        using var binding = session.CreateIoBinding();
        binding.BindInput(0, input);
        binding.BindOutput(0, output);

        session.Run(binding);
    }

    [TestMethod]
    public void OrtValuesCanBeReboundAsInputs()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateTwoInputSession(environment);
        using var firstInput = new OrtTensor<float>([3], [1]);
        using var secondInput = new OrtTensor<float>([7], [1]);
        var values = session.Run(
        [
            session.CreateInputBinding(0, firstInput),
            session.CreateInputBinding(1, secondInput),
        ]);
        using var firstValue = values[0];
        using var secondValue = values[1];
        using var firstOutput = new OrtTensor<float>(new float[1], [1]);
        using var secondOutput = new OrtTensor<float>(new float[1], [1]);
        using var binding = session.CreateIoBinding();
        binding.BindInput(0, firstValue);
        binding.BindInput(1, secondValue);
        binding.BindOutput(0, firstOutput);
        binding.BindOutput(1, secondOutput);

        session.Run(binding);

        Assert.AreEqual(3f, firstOutput.Data[0]);
        Assert.AreEqual(7f, secondOutput.Data[0]);
    }

    [TestMethod]
    public void BindingArgumentsAndDisposalAreValidated()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();
        using var memoryInfo = OrtMemoryInfo.CreateCpu();
        var binding = session.CreateIoBinding();

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => binding.BindInput(-1, input));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => binding.BindOutput(1, output));
        Assert.ThrowsExactly<ArgumentNullException>(() => binding.BindOutputToDevice(0, null!));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => binding.BindOutputToDevice(1, memoryInfo));
        binding.Dispose();
        Assert.ThrowsExactly<ObjectDisposedException>(() => binding.ClearInputs());
    }

    [TestMethod]
    public void DisposedValueBindingFailsWithoutLeakingReference()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        var input = TestData.CreateMnistInput();
        using var binding = session.CreateIoBinding();
        input.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => binding.BindInput(0, input));
    }

    [TestMethod]
    public void SessionRejectsBindingFromAnotherSession()
    {
        using var environment = new OrtEnvironment();
        using var firstSession = TestData.CreateMnistSession(environment);
        using var secondSession = TestData.CreateMnistSession(environment);
        using var binding = firstSession.CreateIoBinding();

        Assert.ThrowsExactly<ArgumentException>(() => secondSession.Run(binding));
    }
}
