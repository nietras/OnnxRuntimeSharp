using System;
using System.IO;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtEnvTest
{
    [TestMethod]
    public unsafe void NativeEnvironmentHandleHasPointerSize()
    {
        Assert.AreEqual(IntPtr.Size, sizeof(Ort.OrtEnvHandle));
    }

    [TestMethod]
    public void NativeEnvironmentHandlePreservesPointer()
    {
        var environment = new Ort.OrtEnvHandle(new IntPtr(42));
        Assert.AreEqual(new IntPtr(42), environment.Value);
        Assert.IsFalse(environment.IsNull);
        Assert.IsTrue(default(Ort.OrtEnvHandle).IsNull);
    }

    [TestMethod]
    public void AvailableExecutionProvidersIncludeCpu()
    {
        CollectionAssert.Contains(TestData.AvailableExecutionProviders.ToList(), "CPUExecutionProvider");
    }

    [TestMethod]
    public void ExecutionProviderDevicesExposeValidMetadata()
    {
        using var environment = new OrtEnv(loggingLevel: Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR);

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
        using var environment = new OrtEnv();

        environment.SetLogLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR);
    }

    [TestMethod]
    public void DisposedEnvironmentRejectsOperations()
    {
        var environment = new OrtEnv();
        environment.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => environment.GetExecutionProviderDevices());
    }

    [TestMethod]
    public void ExecutionProviderLibraryArgumentsAreValidated()
    {
        using var environment = new OrtEnv();

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
        using var environment = new OrtEnv();

        var exception = Assert.ThrowsExactly<OrtException>(() =>
            environment.RegisterExecutionProviderLibrary(
                "missing-test-provider",
                Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.dll")));

        Assert.AreNotEqual(Ort.OrtErrorCode.ORT_OK, exception.ErrorCode);
    }

    [TestMethod]
    public void MissingExecutionProviderRegistrationReturnsStructuredError()
    {
        using var environment = new OrtEnv();

        var exception = Assert.ThrowsExactly<OrtException>(() =>
            environment.UnregisterExecutionProviderLibrary($"missing-{Guid.NewGuid():N}"));

        Assert.AreNotEqual(Ort.OrtErrorCode.ORT_OK, exception.ErrorCode);
    }

    [TestMethod]
    public void DisposedEnvironmentRejectsLogLevelChanges()
    {
        var environment = new OrtEnv();
        environment.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() =>
            environment.SetLogLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR));
    }
}
