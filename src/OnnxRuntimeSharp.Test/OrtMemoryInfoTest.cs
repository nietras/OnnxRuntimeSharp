using System;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtMemoryInfoTest
{
    [TestMethod]
    public void CpuMemoryInfoCanBeQueried()
    {
        using var memoryInfo = OrtMemoryInfo.CreateCpu();

        Assert.IsFalse(string.IsNullOrWhiteSpace(memoryInfo.GetName()));
        Assert.AreEqual(0, memoryInfo.GetDeviceId());
        Assert.AreEqual(Ort.OrtMemType.OrtMemTypeDefault, memoryInfo.GetMemoryType());
        Assert.AreEqual(Ort.OrtAllocatorType.OrtArenaAllocator, memoryInfo.GetAllocatorType());
    }

    [TestMethod]
    public void ExplicitCpuMemoryInfoCanBeQueried()
    {
        using var memoryInfo = new OrtMemoryInfo(
            "Cpu",
            Ort.OrtAllocatorType.OrtDeviceAllocator,
            0,
            Ort.OrtMemType.OrtMemTypeCPUInput);

        Assert.AreEqual("Cpu", memoryInfo.GetName());
        Assert.AreEqual(0, memoryInfo.GetDeviceId());
        Assert.AreEqual(Ort.OrtAllocatorType.OrtDeviceAllocator, memoryInfo.GetAllocatorType());
        Assert.AreEqual(Ort.OrtMemType.OrtMemTypeCPUInput, memoryInfo.GetMemoryType());
    }

    [TestMethod]
    public void MemoryInfoArgumentsAndDisposalAreValidated()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            new OrtMemoryInfo("", Ort.OrtAllocatorType.OrtArenaAllocator, 0, Ort.OrtMemType.OrtMemTypeDefault));

        var memoryInfo = OrtMemoryInfo.CreateCpu();
        memoryInfo.Dispose();
        Assert.ThrowsExactly<ObjectDisposedException>(() => memoryInfo.GetName());
        Assert.ThrowsExactly<ObjectDisposedException>(() => memoryInfo.GetDeviceId());
        Assert.ThrowsExactly<ObjectDisposedException>(() => memoryInfo.GetMemoryType());
        Assert.ThrowsExactly<ObjectDisposedException>(() => memoryInfo.GetAllocatorType());
    }
}
