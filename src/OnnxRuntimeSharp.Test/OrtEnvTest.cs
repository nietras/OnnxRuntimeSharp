using System;
using System.IO;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtEnvTest
{
    [TestMethod]
    public void InstanceReturnsSameEnvironment()
    {
        var env = OrtEnv.Instance();
        var repeated = OrtEnv.Instance();

        Assert.AreSame(env, repeated);
        Assert.IsFalse(env.IsClosed);
        Assert.IsFalse(env.IsInvalid);
    }

    [TestMethod]
    public void DisposingExplicitEnvironmentDoesNotCloseInstance()
    {
        var env = OrtEnv.Instance();
        using (var explicitEnv = new OrtEnv())
        {
            Assert.AreNotSame(env, explicitEnv);
        }
        var repeated = OrtEnv.Instance();
        using var session = new OrtSession(env, TestOnnxModels.Add);

        Assert.AreSame(env, repeated);
        Assert.IsFalse(env.IsClosed);
        Assert.HasCount(2, session.Inputs);
        Assert.HasCount(1, session.Outputs);
    }

    [TestMethod]
    public unsafe void NativeEnvironmentHandleHasPointerSize()
    {
        Assert.AreEqual(IntPtr.Size, sizeof(Ort.OrtEnvHandle));
    }

    [TestMethod]
    public void NativeEnvironmentHandlePreservesPointer()
    {
        var envHandle = new Ort.OrtEnvHandle(new IntPtr(42));
        Assert.AreEqual(new IntPtr(42), envHandle.Value);
        Assert.IsFalse(envHandle.IsNull);
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
        TestData.RequirePluginApi();
        using var env = new OrtEnv(loggingLevel: Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR);

        var devices = env.GetExecutionProviderDevices();

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
        using var env = new OrtEnv();
        env.SetLogLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR);
    }

    [TestMethod]
    public void DisposedEnvironmentRejectsOperations()
    {
        var env = new OrtEnv();
        env.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => env.GetExecutionProviderDevices());
    }

    [TestMethod]
    public void ExecutionProviderLibraryArgumentsAreValidated()
    {
        using var env = new OrtEnv();

        Assert.ThrowsExactly<ArgumentException>(() =>
            env.RegisterExecutionProviderLibrary("", "provider.dll"));
        Assert.ThrowsExactly<ArgumentException>(() =>
            env.RegisterExecutionProviderLibrary("provider", ""));
        Assert.ThrowsExactly<ArgumentException>(() =>
            env.UnregisterExecutionProviderLibrary(""));
    }

    [TestMethod]
    public void MissingExecutionProviderLibraryReturnsStructuredError()
    {
        TestData.RequirePluginApi();
        using var env = new OrtEnv();

        var exception = Assert.ThrowsExactly<OrtException>(() =>
            env.RegisterExecutionProviderLibrary(
                "missing-test-provider",
                Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.dll")));

        Assert.AreNotEqual(Ort.OrtErrorCode.ORT_OK, exception.ErrorCode);
    }

    [TestMethod]
    public void MissingExecutionProviderRegistrationReturnsStructuredError()
    {
        TestData.RequirePluginApi();
        using var env = new OrtEnv();

        var exception = Assert.ThrowsExactly<OrtException>(() =>
            env.UnregisterExecutionProviderLibrary($"missing-{Guid.NewGuid():N}"));

        Assert.AreNotEqual(Ort.OrtErrorCode.ORT_OK, exception.ErrorCode);
    }

    [TestMethod]
    public void DisposedEnvironmentRejectsLogLevelChanges()
    {
        var env = new OrtEnv();
        env.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() =>
            env.SetLogLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR));
    }
}
