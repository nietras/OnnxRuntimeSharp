using System;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtEpDeviceTest
{
    [TestMethod]
    public void DeviceCanBeAppendedThroughPluginApi()
    {
        using var environment = new OrtEnvironment();
        var device = environment.GetExecutionProviderDevices()
            .First(item => string.Equals(
                item.ExecutionProviderName,
                "CPUExecutionProvider",
                StringComparison.Ordinal));
        using var options = new OrtSessionOptions();

        options.AppendExecutionProvider(environment, [device]);
        using var session = TestData.CreateMnistSession(environment, options);

        Assert.HasCount(1, session.Inputs);
    }

    [TestMethod]
    public void HardwareDeviceInfoIsStableAcrossEnumeration()
    {
        using var environment = new OrtEnvironment();
        var first = environment.GetExecutionProviderDevices()[0].HardwareDevice;
        var second = environment.GetExecutionProviderDevices()[0].HardwareDevice;

        Assert.AreEqual(first.Type, second.Type);
        Assert.AreEqual(first.VendorId, second.VendorId);
        Assert.AreEqual(first.Vendor, second.Vendor);
        Assert.AreEqual(first.DeviceId, second.DeviceId);
    }
}
