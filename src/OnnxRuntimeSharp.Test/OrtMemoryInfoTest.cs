using System;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtMemoryInfoTest
{
    [TestMethod]
    public void CpuMemoryInfoExposesProperties()
    {
        using var memoryInfo = OrtMemoryInfo.CreateCpu();

        Assert.IsFalse(string.IsNullOrWhiteSpace(memoryInfo.Name));
        Assert.AreEqual(0, memoryInfo.DeviceId);
        Assert.AreEqual(Ort.OrtMemType.OrtMemTypeDefault, memoryInfo.MemoryType);
        Assert.AreEqual(Ort.OrtAllocatorType.OrtArenaAllocator, memoryInfo.AllocatorType);
    }

    [TestMethod]
    public void ExplicitCpuMemoryInfoExposesProperties()
    {
        using var memoryInfo = new OrtMemoryInfo(
            "Cpu",
            Ort.OrtAllocatorType.OrtDeviceAllocator,
            0,
            Ort.OrtMemType.OrtMemTypeCPUInput);

        Assert.AreEqual("Cpu", memoryInfo.Name);
        Assert.AreEqual(Ort.OrtAllocatorType.OrtDeviceAllocator, memoryInfo.AllocatorType);
        Assert.AreEqual(Ort.OrtMemType.OrtMemTypeCPUInput, memoryInfo.MemoryType);
    }

    [TestMethod]
    public void MemoryInfoArgumentsAndDisposalAreValidated()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            new OrtMemoryInfo("", Ort.OrtAllocatorType.OrtArenaAllocator, 0, Ort.OrtMemType.OrtMemTypeDefault));

        var memoryInfo = OrtMemoryInfo.CreateCpu();
        memoryInfo.Dispose();
        Assert.ThrowsExactly<ObjectDisposedException>(() => _ = memoryInfo.Name);
    }
}
