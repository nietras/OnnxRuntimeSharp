using System;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtEpProbeTest
{
    [TestMethod]
    public void SingleProviderProbeReturnsSuccessfulConfiguration()
    {
        using var environment = new OrtEnv();

        var result = OrtEpConfigs.ProbeExecutionProvider(environment, OrtEpConfigs.CPUSingleThread);

        Assert.AreSame(OrtEpConfigs.CPUSingleThread, result.Provider);
        Assert.IsTrue(result.IsAvailable);
        Assert.IsNull(result.Error);
        Assert.IsFalse(environment.IsClosed);
    }

    [TestMethod]
    public void SingleProviderProbePreservesAppendFailure()
    {
        using var environment = new OrtEnv();
        var failure = new InvalidOperationException("Test provider failure.");
        var provider = new OrtEpConfig("Broken", _ => throw failure);

        var result = OrtEpConfigs.ProbeExecutionProvider(environment, provider);

        Assert.AreSame(provider, result.Provider);
        Assert.IsFalse(result.IsAvailable);
        Assert.AreSame(failure, result.Error);
    }
}
