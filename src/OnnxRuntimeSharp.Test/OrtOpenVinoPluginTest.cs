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
            Assert.Inconclusive(
                $"OpenVINO is unavailable on this machine. Could not load '{libraryPath}'.");
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
                var providerNames = allDevices.Select(device => device.ExecutionProviderName);
                var availableProviders = string.Join(", ", providerNames);
                Assert.Inconclusive(
                    "OpenVINO is unavailable on this machine. " +
                    $"Registered '{executionProviderName}', " +
                    $"but available providers were: {availableProviders}. ");
            }

            var device = devices.FirstOrDefault(item =>
                item.HardwareDevice.Type == Ort.OrtHardwareDeviceType.OrtHardwareDeviceType_CPU) ??
                devices[0];
            using var options = new OrtSessionOptions();
            // A successful run must execute Add on OpenVINO, not silently fall back to CPU.
            options.AddConfigEntry("session.disable_cpu_ep_fallback", "1");
            options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_DISABLE_ALL);
            options.AppendExecutionProvider(environment, [device]);
            using var session = new OrtSession(environment, TestOnnxModels.Add, options);
            AssertAddOutputs(session);
        }
        finally
        {
            environment.UnregisterExecutionProviderLibrary(OpenVINORegistrationName);
        }
    }

    [TestMethod]
    public void AddProbeModelProducesExpectedOutputsOnCpu()
    {
        using var environment = new OrtEnv();
        using var session = new OrtSession(environment, TestOnnxModels.Add);
        AssertAddOutputs(session);
    }

    [TestMethod]
    public void AddProbeModelRejectsCpuFallbackWhenNoProviderIsAppended()
    {
        using var environment = new OrtEnv();
        using var options = new OrtSessionOptions();
        // Session creation is expected to fail; keep the exception without logging the error.
        options.SetLogSeverityLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_FATAL);
        options.AddConfigEntry("session.disable_cpu_ep_fallback", "1");
        options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_DISABLE_ALL);

        Assert.ThrowsExactly<OrtException>(() =>
        {
            using var session = new OrtSession(environment, TestOnnxModels.Add, options);
        });
    }

    static void AssertAddOutputs(OrtSession session)
    {
        using var first = new OrtValue<float>(new float[] { 2 }, [1]);
        using var second = new OrtValue<float>(new float[] { 3 }, [1]);
        using var output = new OrtValue<float>(new float[1], [1]);
        var inputs = new[]
        {
            session.CreateInputBinding(0, first), session.CreateInputBinding(1, second)
        };
        var outputs = new[] { session.CreateOutputBinding(0, output) };

        session.Run(inputs, outputs);
        Assert.AreEqual(5f, output.Data[0]);

        first.Data[0] = -4;
        session.Run(inputs, outputs);
        Assert.AreEqual(-1f, output.Data[0]);
    }
}
