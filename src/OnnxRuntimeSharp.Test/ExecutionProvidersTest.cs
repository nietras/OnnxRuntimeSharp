using System;
using System.Collections.Generic;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class ExecutionProvidersTest
{
    const string CPUName = "CPU";
    const string TestProviderName = "test";

    [TestMethod]
    public void DefaultPriorityIsStable()
    {
        var names = ExecutionProviders.DefaultPrioritizedList
            .Select(provider => provider.Name).ToArray();

        Assert.AreSequenceEqual(
            [ExecutionProviders.TensorRTName, ExecutionProviders.CUDAName,
             ExecutionProviders.DirectMLName, ExecutionProviders.OpenVINOName,
             CPUName, "None"], names);
    }

    [TestMethod]
    public void CpuConfigurationsExecuteProbeAndPreservePriority()
    {
        var candidates = new[]
        {
            ExecutionProviders.CPUSingleThread, ExecutionProviders.CPU, ExecutionProviders.None
        };
        var available = ExecutionProviders.FindAvailablePrioritizedExecutionProviders(candidates);

        Assert.AreSequenceEqual(candidates, available);
    }

    [TestMethod]
    public void FailedConfigurationDoesNotHideOtherProviders()
    {
        var failure = new InvalidOperationException("test configuration failure");
        var broken = new ExecutionProvider("Broken", _ => throw failure);
        // Session creation is expected to fail; suppress its native error log, not the exception.
        var fallbackOnly = new ExecutionProvider("Not an accelerator",
            options => options.SetLogSeverityLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_FATAL));
        using var environment = new OrtEnv();
        var candidates = new[]
        {
            broken, ExecutionProviders.CPU, fallbackOnly, ExecutionProviders.CPUSingleThread
        };
        var results = ExecutionProviders.ProbeExecutionProviders(environment, candidates);
        var availableProviders = ExecutionProviders.FindAvailablePrioritizedExecutionProviders(
            environment, candidates);

        Assert.HasCount(4, results);
        Assert.AreSame(failure, results[0].Error);
        Assert.IsFalse(results[0].IsAvailable);
        Assert.IsTrue(results[1].IsAvailable);
        Assert.IsInstanceOfType<OrtException>(results[2].Error);
        Assert.IsFalse(results[2].IsAvailable);
        Assert.IsTrue(results[3].IsAvailable);
        Assert.AreSequenceEqual(
            [ExecutionProviders.CPU, ExecutionProviders.CPUSingleThread], availableProviders);
    }

    [TestMethod]
    public void QueriesAreEagerAndCanBeRepeated()
    {
        var appendCount = 0;
        var customCpu = new ExecutionProvider("Custom CPU", _ => ++appendCount, allowsCpuFallback: true);
        using var environment = new OrtEnv();
        var available = ExecutionProviders.FindAvailablePrioritizedExecutionProviders(
            environment, [customCpu]);
        Assert.AreEqual(1, appendCount);
        Assert.AreSame(customCpu, available[0]);
        var repeated = ExecutionProviders.ProbeExecutionProviders(environment, [customCpu]);
        Assert.IsTrue(repeated[0].IsAvailable);
        Assert.AreEqual(2, appendCount);
        var empty = ExecutionProviders.ProbeExecutionProviders(environment, []);
        Assert.IsEmpty(empty);
    }

    [TestMethod]
    public void QueriesWithoutEnvironmentReuseInstance()
    {
        var environment = OrtEnv.Instance();
        var observedEnvironments = new List<OrtEnv>();
        var customCpu = new ExecutionProvider("Shared CPU",
            (env, _) => observedEnvironments.Add(env), allowsCpuFallback: true);

        var available = ExecutionProviders.FindAvailablePrioritizedExecutionProviders([customCpu]);
        var results = ExecutionProviders.ProbeExecutionProviders([customCpu]);

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
        var results = ExecutionProviders.ProbeExecutionProviders();
        var cpu = results.Single(result => result.Provider == ExecutionProviders.CPU);
        var none = results.Single(result => result.Provider == ExecutionProviders.None);

        Assert.HasCount(ExecutionProviders.DefaultPrioritizedList.Count, results);
        Assert.IsTrue(cpu.IsAvailable);
        Assert.IsTrue(none.IsAvailable);
        if (Ort.Api->SessionOptionsAppendExecutionProvider_OpenVINO_V2 == null)
        {
            var openVino = results.Single(result => result.Provider == ExecutionProviders.OpenVINO);
            Assert.IsInstanceOfType<NotSupportedException>(openVino.Error);
        }
    }

    [TestMethod]
    public void OpenVinoProbeCanRunRepeatedlyWhenSupported()
    {
        using var environment = new OrtEnv();
        var results = ExecutionProviders.ProbeExecutionProviders(
            environment, [ExecutionProviders.OpenVINO]);
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
        var repeated = ExecutionProviders.ProbeExecutionProviders(
            environment, [ExecutionProviders.OpenVINO]);
        Assert.IsTrue(repeated[0].IsAvailable, repeated[0].Error?.ToString());
        var configured = ExecutionProviders.CreateOpenVINO(
            new Dictionary<string, string> { ["device_type"] = CPUName });
        var configuredResults = ExecutionProviders.ProbeExecutionProviders(environment, [configured]);
        var configuredResult = configuredResults[0];
        Assert.IsTrue(configuredResult.IsAvailable, configuredResult.Error?.ToString());
        var available = ExecutionProviders.FindAvailablePrioritizedExecutionProviders(
            environment, [ExecutionProviders.OpenVINO]);
        Assert.AreSequenceEqual([ExecutionProviders.OpenVINO], available);
    }

    [TestMethod]
    public void ArgumentsAreValidated()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new ExecutionProvider("", _ => { }));
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new ExecutionProvider(TestProviderName, (Action<OrtSessionOptions>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new ExecutionProvider(TestProviderName, (Action<OrtEnv, OrtSessionOptions>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(
            () => ExecutionProviders.ProbeExecutionProviders((OrtEnv)null!, []));
        using var environment = new OrtEnv();
        var results = ExecutionProviders.ProbeExecutionProviders(environment, [null!]);
        var invalidCandidate = results[0];
        Assert.IsFalse(invalidCandidate.IsAvailable);
        Assert.IsNotNull(invalidCandidate.Error);
        environment.Dispose();
        Assert.ThrowsExactly<ObjectDisposedException>(
            () => ExecutionProviders.ProbeExecutionProviders(environment));
    }
}
