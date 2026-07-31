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
}
