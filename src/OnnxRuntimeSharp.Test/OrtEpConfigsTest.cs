using System;
using System.Collections.Generic;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtEpConfigsTest
{
    const string CPUName = "CPU";
    const string TestProviderName = "test";

    [TestMethod]
    public void DefaultPriorityIsStable()
    {
        var names = OrtEpConfigs.DefaultPrioritizedList
            .Select(provider => provider.Name).ToArray();

        Assert.AreSequenceEqual(
            [OrtEpConfigs.TensorRTName, OrtEpConfigs.CUDAName,
             OrtEpConfigs.DirectMLName, OrtEpConfigs.OpenVINOName,
             CPUName, "None"], names);
    }

    [TestMethod]
    public void CpuConfigurationsExecuteProbeAndPreservePriority()
    {
        var candidates = new[]
        {
            OrtEpConfigs.CPUSingleThread, OrtEpConfigs.CPU, OrtEpConfigs.None
        };
        var available = OrtEpConfigs.FindAvailablePrioritizedExecutionProviders(candidates);

        Assert.AreSequenceEqual(candidates, available);
    }

    [TestMethod]
    public void FailedConfigurationDoesNotHideOtherProviders()
    {
        var failure = new InvalidOperationException("test configuration failure");
        var broken = new OrtEpConfig("Broken", _ => throw failure);
        // Session creation is expected to fail; suppress its native error log, not the exception.
        var fallbackOnly = new OrtEpConfig("Not an accelerator",
            options => options.SetLogSeverityLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_FATAL));
        using var environment = new OrtEnv();
        var candidates = new[]
        {
            broken, OrtEpConfigs.CPU, fallbackOnly, OrtEpConfigs.CPUSingleThread
        };
        var results = OrtEpConfigs.ProbeExecutionProviders(environment, candidates);
        var availableProviders = OrtEpConfigs.FindAvailablePrioritizedExecutionProviders(
            environment, candidates);

        Assert.HasCount(4, results);
        Assert.AreSame(failure, results[0].Error);
        Assert.IsFalse(results[0].IsAvailable);
        Assert.IsTrue(results[1].IsAvailable);
        Assert.IsInstanceOfType<OrtException>(results[2].Error);
        Assert.IsFalse(results[2].IsAvailable);
        Assert.IsTrue(results[3].IsAvailable);
        Assert.AreSequenceEqual(
            [OrtEpConfigs.CPU, OrtEpConfigs.CPUSingleThread], availableProviders);
    }

    [TestMethod]
    public void QueriesAreEagerAndCanBeRepeated()
    {
        var appendCount = 0;
        var customCpu = new OrtEpConfig("Custom CPU", _ => ++appendCount, allowsCpuFallback: true);
        using var environment = new OrtEnv();
        var available = OrtEpConfigs.FindAvailablePrioritizedExecutionProviders(
            environment, [customCpu]);
        Assert.AreEqual(1, appendCount);
        Assert.AreSame(customCpu, available[0]);
        var repeated = OrtEpConfigs.ProbeExecutionProviders(environment, [customCpu]);
        Assert.IsTrue(repeated[0].IsAvailable);
        Assert.AreEqual(2, appendCount);
        var empty = OrtEpConfigs.ProbeExecutionProviders(environment, []);
        Assert.IsEmpty(empty);
    }

    [TestMethod]
    public void QueriesWithoutEnvironmentReuseInstance()
    {
        var environment = OrtEnv.Instance();
        var observedEnvironments = new List<OrtEnv>();
        var customCpu = new OrtEpConfig("Shared CPU",
            (env, _) => observedEnvironments.Add(env), allowsCpuFallback: true);

        var available = OrtEpConfigs.FindAvailablePrioritizedExecutionProviders([customCpu]);
        var results = OrtEpConfigs.ProbeExecutionProviders([customCpu]);

        Assert.AreSequenceEqual([customCpu], available);
        Assert.IsTrue(results[0].IsAvailable);
        Assert.HasCount(2, observedEnvironments);
        Assert.AreSame(environment, observedEnvironments[0]);
        Assert.AreSame(environment, observedEnvironments[1]);
        Assert.IsFalse(environment.IsClosed);
    }

    [TestMethod]
    public unsafe void DefaultQueryIncludesCpuAndHandlesMissingProviders()
    {
        var results = OrtEpConfigs.ProbeExecutionProviders();
        var cpu = results.Single(result => result.Provider == OrtEpConfigs.CPU);
        var none = results.Single(result => result.Provider == OrtEpConfigs.None);

        Assert.HasCount(OrtEpConfigs.DefaultPrioritizedList.Count, results);
        Assert.IsTrue(cpu.IsAvailable);
        Assert.IsTrue(none.IsAvailable);
        if (Ort.Api->SessionOptionsAppendExecutionProvider_OpenVINO_V2 == null)
        {
            var openVino = results.Single(result => result.Provider == OrtEpConfigs.OpenVINO);
            Assert.IsInstanceOfType<NotSupportedException>(openVino.Error);
        }
    }

    [TestMethod]
    public void OpenVinoProbeCanRunRepeatedlyWhenSupported()
    {
        using var environment = new OrtEnv();
        var results = OrtEpConfigs.ProbeExecutionProviders(
            environment, [OrtEpConfigs.OpenVINO]);
        var result = results[0];
        if (result.Error is OrtException error &&
            (error.Message.Contains("Failed to load") || error.Message.Contains("LoadLibrary failed")))
        {
            Assert.Inconclusive($"OpenVINO is unavailable on this machine: {error.Message}");
        }
        if (result.Error is NotSupportedException unavailable)
        {
            Assert.Inconclusive(unavailable.Message);
        }
        Assert.IsTrue(result.IsAvailable, result.Error?.ToString());
        var repeated = OrtEpConfigs.ProbeExecutionProviders(
            environment, [OrtEpConfigs.OpenVINO]);
        Assert.IsTrue(repeated[0].IsAvailable, repeated[0].Error?.ToString());
        var configured = OrtEpConfigs.CreateOpenVINO("OpenVINO CPU",
            new Dictionary<string, string> { ["device_type"] = CPUName });
        var configuredResults = OrtEpConfigs.ProbeExecutionProviders(environment, [configured]);
        var configuredResult = configuredResults[0];
        Assert.IsTrue(configuredResult.IsAvailable, configuredResult.Error?.ToString());
        var available = OrtEpConfigs.FindAvailablePrioritizedExecutionProviders(
            environment, [OrtEpConfigs.OpenVINO]);
        Assert.AreSequenceEqual([OrtEpConfigs.OpenVINO], available);
    }

    [TestMethod]
    public void ArgumentsAreValidated()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new OrtEpConfig("", _ => { }));
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new OrtEpConfig(TestProviderName, (Action<OrtSessionOptions>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new OrtEpConfig(TestProviderName, (Action<OrtEnv, OrtSessionOptions>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(
            () => OrtEpConfigs.ProbeExecutionProviders(null!, []));
        using var environment = new OrtEnv();
        var results = OrtEpConfigs.ProbeExecutionProviders(environment, [null!]);
        var invalidCandidate = results[0];
        Assert.IsFalse(invalidCandidate.IsAvailable);
        Assert.IsNotNull(invalidCandidate.Error);
        environment.Dispose();
        Assert.ThrowsExactly<ObjectDisposedException>(
            () => OrtEpConfigs.ProbeExecutionProviders(environment));
    }
}
