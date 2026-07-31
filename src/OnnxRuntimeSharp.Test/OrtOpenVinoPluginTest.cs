using System;
using System.Linq;
using Intel.ML.OnnxRuntime.EP.OpenVINO;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtOpenVinoPluginTest
{
    [TestMethod]
    public void OpenVinoPluginCanRegisterEnumerateAndRunMnist()
    {
        const string RegistrationName = "openvino_ep_registration";
        var executionProviderName = OpenVINOEp.GetEpName();
        using var environment = new OrtEnvironment();
        environment.RegisterExecutionProviderLibrary(
            RegistrationName,
            OpenVINOEp.GetLibraryPath());
        try
        {
            var allDevices = environment.GetExecutionProviderDevices();
            var devices = allDevices
                .Where(device => string.Equals(
                    device.ExecutionProviderName,
                    executionProviderName,
                    StringComparison.Ordinal))
                .ToArray();
            Assert.IsNotEmpty(
                devices,
                $"Registered '{executionProviderName}', but available devices were: " +
                string.Join(", ", allDevices.Select(device => device.ExecutionProviderName)));

            var device = devices.FirstOrDefault(item =>
                item.HardwareDevice.Type == Ort.OrtHardwareDeviceType.OrtHardwareDeviceType_CPU) ??
                devices[0];
            using var options = new OrtSessionOptions();
            options.AppendExecutionProvider(environment, [device]);
            using var session = TestData.CreateMnistSession(environment, options);
            using var input = TestData.CreateMnistInput();
            using var output = TestData.CreateMnistOutput();

            session.Run(input, output);

            foreach (var value in output.Data)
            {
                Assert.IsTrue(float.IsFinite(value));
            }
        }
        finally
        {
            environment.UnregisterExecutionProviderLibrary(RegistrationName);
        }
    }
}
