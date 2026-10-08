using System;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtOpenVinoPluginTest
{
    const string OpenVINORegistrationName = "openvino_ep_registration";
    const string OpenVINOExecutionProviderName = "OpenVINOExecutionProvider";
    const string OpenVINOLibraryPath = "onnxruntime_providers_openvino.dll";

    [TestMethod]
    public void OpenVinoPluginCanRegisterAndRunWhenSupported()
    {
        TestData.RequirePluginApi();
        var registrationName = OpenVINORegistrationName;
        var libraryPath = OpenVINOLibraryPath;
        var executionProviderName = OpenVINOExecutionProviderName;
        using var environment = new OrtEnv();
        try
        {
            environment.RegisterExecutionProviderLibrary(registrationName, libraryPath);
        }
        catch (OrtException ex) when (ex.Message.Contains("Failed to load"))
        {
            Assert.Inconclusive($"OpenVINO is unavailable on this machine. Could not load '{libraryPath}'.");
        }
        try
        {
            var allDevices = environment.GetExecutionProviderDevices();
            var devices = allDevices
                .Where(device => string.Equals(
                    device.ExecutionProviderName,
                    executionProviderName,
                    StringComparison.Ordinal))
                .ToArray();
            if (devices.Length == 0)
            {
                Assert.Inconclusive(
                    $"OpenVINO is unavailable on this machine. Registered '{executionProviderName}', " +
                    $"but available providers were: {string.Join(", ", allDevices.Select(device => device.ExecutionProviderName))}. " +
                    "OpenVINO requires a supported Intel device; x64 alone is not sufficient.");
            }

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
            environment.UnregisterExecutionProviderLibrary(OpenVINORegistrationName);
        }
    }
}
