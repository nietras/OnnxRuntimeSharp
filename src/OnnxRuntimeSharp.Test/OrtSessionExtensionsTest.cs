namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtSessionExtensionsTest
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(3)]
    public void WarmupLeavesMultiInputOutputSessionUsable(int warmupRuns)
    {
        using var environment = new OrtEnv();
        using var session = TestData.CreateTwoInputSession(environment);

        session.Warmup(warmupRuns);

        using var first = new OrtValue<float>([3], [1]);
        using var second = new OrtValue<float>([7], [1]);
        using var firstOutput = new OrtValue<float>(new float[1], [1]);
        using var secondOutput = new OrtValue<float>(new float[1], [1]);
        session.Run(
            [session.CreateInputBinding(0, first), session.CreateInputBinding(1, second)],
            [session.CreateOutputBinding(0, firstOutput), session.CreateOutputBinding(1, secondOutput)]);

        Assert.AreEqual(3f, firstOutput.Data[0]);
        Assert.AreEqual(7f, secondOutput.Data[0]);
        Assert.AreSequenceEqual([1], session.Inputs.Span[0].Dimensions.ToArray());
    }

    [TestMethod]
    public void DisabledWarmupDoesNotCreateTensors()
    {
        using var environment = new OrtEnv();
        using var session = TestData.CreateTwoInputSession(environment);

        session.Warmup(warmupRuns: 0, warmupBatchSize: -1);
    }
}
