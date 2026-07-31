using System;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtEnvironmentTest
{
    [TestMethod]
    public void AvailableExecutionProvidersIncludeCpu()
    {
        CollectionAssert.Contains(TestData.AvailableExecutionProviders.ToList(), "CPUExecutionProvider");
    }

    [TestMethod]
    public void ExecutionProviderDevicesExposeValidMetadata()
    {
        using var environment = new OrtEnvironment(loggingLevel: Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR);

        var devices = environment.GetExecutionProviderDevices();

        Assert.IsNotEmpty(devices);
        foreach (var device in devices)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(device.ExecutionProviderName));
            Assert.IsFalse(string.IsNullOrWhiteSpace(device.HardwareDevice.Vendor));
            Assert.IsTrue(Enum.IsDefined(device.HardwareDevice.Type));
        }
    }

    [TestMethod]
    public void LogLevelCanBeChanged()
    {
        using var environment = new OrtEnvironment();

        environment.SetLogLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR);
    }

    [TestMethod]
    public void DisposedEnvironmentRejectsOperations()
    {
        var environment = new OrtEnvironment();
        environment.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => environment.GetExecutionProviderDevices());
    }
}
