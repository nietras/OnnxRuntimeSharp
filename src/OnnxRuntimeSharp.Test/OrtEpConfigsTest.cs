using System;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtEpConfigsTest
{
    [TestMethod]
    public void DefaultPriorityIsStable()
    {
        var names = OrtEpConfigs.DefaultPrioritizedList
            .Select(provider => provider.Name).ToArray();

        Assert.AreSequenceEqual(
            [OrtEpConfigs.TensorRTName, OrtEpConfigs.CUDAName,
             OrtEpConfigs.DirectMLName, OrtEpConfigs.OpenVINOName,
             "CPU", "None"], names);
    }

    [TestMethod]
    public void ConfigurationArgumentsAreValidated()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new OrtEpConfig("", _ => { }));
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new OrtEpConfig("test", (Action<OrtSessionOptions>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new OrtEpConfig("test", (Action<OrtEnv, OrtSessionOptions>)null!));
    }
}
