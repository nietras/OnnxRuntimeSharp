using System;
using System.Collections.Generic;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtEpProbingTest
{
    const string CPUName = "CPU";

    [TestMethod]
    public void SingleProviderProbeReturnsSuccessfulConfiguration()
    {
        using var env = new OrtEnv();

        var result = OrtEpProbing.ProbeExecutionProvider(env, OrtEpConfigs.CPUSingleThread);

        Assert.AreSame(OrtEpConfigs.CPUSingleThread, result.Provider);
        Assert.IsTrue(result.IsAvailable);
        Assert.IsNull(result.Error);
        Assert.IsFalse(env.IsClosed);
    }

    [TestMethod]
    public void SingleProviderProbePreservesAppendFailure()
    {
        using var env = new OrtEnv();
        var failure = new InvalidOperationException("Test provider failure.");
        var provider = new OrtEpConfig("Broken", _ => throw failure);

        var result = OrtEpProbing.ProbeExecutionProvider(env, provider);

        Assert.AreSame(provider, result.Provider);
        Assert.IsFalse(result.IsAvailable);
        Assert.AreSame(failure, result.Error);
    }

    [TestMethod]
    public void CpuConfigurationsExecuteProbeAndPreservePriority()
    {
        var candidates = new[]
        {
            OrtEpConfigs.CPUSingleThread, OrtEpConfigs.CPU, OrtEpConfigs.None
        };
        var available = OrtEpProbing.ProbeAvailablePrioritizedExecutionProviders(candidates);

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
        using var env = new OrtEnv();
        var candidates = new[]
        {
            broken, OrtEpConfigs.CPU, fallbackOnly, OrtEpConfigs.CPUSingleThread
        };
        var results = OrtEpProbing.ProbeExecutionProviders(env, candidates);
        var availableProviders = OrtEpProbing.ProbeAvailablePrioritizedExecutionProviders(
            env, candidates);

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
    public void ProbesAreEagerAndCanBeRepeated()
    {
        var appendCount = 0;
        var customCpu = new OrtEpConfig("Custom CPU", _ => ++appendCount, allowsCpuFallback: true);
        using var env = new OrtEnv();
        var available = OrtEpProbing.ProbeAvailablePrioritizedExecutionProviders(
            env, [customCpu]);
        Assert.AreEqual(1, appendCount);
        Assert.AreSame(customCpu, available[0]);
        var repeated = OrtEpProbing.ProbeExecutionProviders(env, [customCpu]);
        Assert.IsTrue(repeated[0].IsAvailable);
        Assert.AreEqual(2, appendCount);
        var empty = OrtEpProbing.ProbeExecutionProviders(env, []);
        Assert.IsEmpty(empty);
    }

    [TestMethod]
    public void ProbesWithoutEnvironmentReuseInstance()
    {
        var env = OrtEnv.Instance;
        var observedEnvs = new List<OrtEnv>();
        var customCpu = new OrtEpConfig("Shared CPU",
            (env, _) => observedEnvs.Add(env), allowsCpuFallback: true);

        var available = OrtEpProbing.ProbeAvailablePrioritizedExecutionProviders([customCpu]);
        var results = OrtEpProbing.ProbeExecutionProviders([customCpu]);
        var result = OrtEpProbing.ProbeExecutionProvider(customCpu);

        Assert.AreSequenceEqual([customCpu], available);
        Assert.IsTrue(results[0].IsAvailable);
        Assert.AreSame(customCpu, result.Provider);
        Assert.IsTrue(result.IsAvailable);
        Assert.IsNull(result.Error);
        Assert.HasCount(3, observedEnvs);
        Assert.AreSame(env, observedEnvs[0]);
        Assert.AreSame(env, observedEnvs[1]);
        Assert.AreSame(env, observedEnvs[2]);
        Assert.IsFalse(env.IsClosed);
    }

    [TestMethod]
    public unsafe void DefaultProbeIncludesCpuAndHandlesMissingProviders()
    {
        var results = OrtEpProbing.ProbeExecutionProviders();
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
        using var env = new OrtEnv();
        var results = OrtEpProbing.ProbeExecutionProviders(
            env, [OrtEpConfigs.OpenVINO]);
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
        var repeated = OrtEpProbing.ProbeExecutionProviders(
            env, [OrtEpConfigs.OpenVINO]);
        Assert.IsTrue(repeated[0].IsAvailable, repeated[0].Error?.ToString());
        var configured = OrtEpConfigs.CreateOpenVINO("OpenVINO CPU",
            new Dictionary<string, string> { ["device_type"] = CPUName });
        var configuredResults = OrtEpProbing.ProbeExecutionProviders(env, [configured]);
        var configuredResult = configuredResults[0];
        Assert.IsTrue(configuredResult.IsAvailable, configuredResult.Error?.ToString());
        var available = OrtEpProbing.ProbeAvailablePrioritizedExecutionProviders(
            env, [OrtEpConfigs.OpenVINO]);
        Assert.AreSequenceEqual([OrtEpConfigs.OpenVINO], available);
    }

    [TestMethod]
    public void ProbeArgumentsAreValidated()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => OrtEpProbing.ProbeExecutionProviders(null!, []));
        using var env = new OrtEnv();
        var results = OrtEpProbing.ProbeExecutionProviders(env, [null!]);
        var invalidCandidate = results[0];
        Assert.IsFalse(invalidCandidate.IsAvailable);
        Assert.IsNotNull(invalidCandidate.Error);
        env.Dispose();
        Assert.ThrowsExactly<ObjectDisposedException>(
            () => OrtEpProbing.ProbeExecutionProviders(env));
    }
}
