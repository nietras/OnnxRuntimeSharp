using System;
using System.IO;
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

    [TestMethod]
    public void ExecutionProviderLibraryArgumentsAreValidated()
    {
        using var environment = new OrtEnvironment();

        Assert.ThrowsExactly<ArgumentException>(() =>
            environment.RegisterExecutionProviderLibrary("", "provider.dll"));
        Assert.ThrowsExactly<ArgumentException>(() =>
            environment.RegisterExecutionProviderLibrary("provider", ""));
        Assert.ThrowsExactly<ArgumentException>(() =>
            environment.UnregisterExecutionProviderLibrary(""));
    }

    [TestMethod]
    public void MissingExecutionProviderLibraryReturnsStructuredError()
    {
        using var environment = new OrtEnvironment();

        var exception = Assert.ThrowsExactly<OrtException>(() =>
            environment.RegisterExecutionProviderLibrary(
                "missing-test-provider",
                Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.dll")));

        Assert.AreNotEqual(Ort.OrtErrorCode.ORT_OK, exception.ErrorCode);
    }

    [TestMethod]
    public void MissingExecutionProviderRegistrationReturnsStructuredError()
    {
        using var environment = new OrtEnvironment();

        var exception = Assert.ThrowsExactly<OrtException>(() =>
            environment.UnregisterExecutionProviderLibrary($"missing-{Guid.NewGuid():N}"));

        Assert.AreNotEqual(Ort.OrtErrorCode.ORT_OK, exception.ErrorCode);
    }

    [TestMethod]
    public void DisposedEnvironmentRejectsLogLevelChanges()
    {
        var environment = new OrtEnvironment();
        environment.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() =>
            environment.SetLogLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR));
    }
}
