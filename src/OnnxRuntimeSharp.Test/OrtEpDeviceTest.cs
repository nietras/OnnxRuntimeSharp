using System;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtEpDeviceTest
{
    [TestMethod]
    public void DeviceCanBeAppendedThroughPluginApi()
    {
        TestData.RequirePluginApi();
        using var env = new OrtEnv();
        var device = env.GetExecutionProviderDevices()
            .First(item => string.Equals(
                item.ExecutionProviderName,
                "CPUExecutionProvider",
                StringComparison.Ordinal));
        using var options = new OrtSessionOptions();

        options.AppendExecutionProvider(env, [device]);
        using var session = TestData.CreateMnistSession(env, options);

        Assert.HasCount(1, session.Inputs);
    }

    [TestMethod]
    public void HardwareDeviceInfoIsStableAcrossEnumeration()
    {
        TestData.RequirePluginApi();
        using var env = new OrtEnv();
        var first = env.GetExecutionProviderDevices()[0].HardwareDevice;
        var second = env.GetExecutionProviderDevices()[0].HardwareDevice;

        Assert.AreEqual(first.Type, second.Type);
        Assert.AreEqual(first.VendorId, second.VendorId);
        Assert.AreEqual(first.Vendor, second.Vendor);
        Assert.AreEqual(first.DeviceId, second.DeviceId);
    }
}
