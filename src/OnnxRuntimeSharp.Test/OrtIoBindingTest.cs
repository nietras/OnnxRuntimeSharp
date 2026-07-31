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
}
