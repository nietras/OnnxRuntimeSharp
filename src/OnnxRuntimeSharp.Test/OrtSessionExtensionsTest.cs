using System;

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
        using var env = new OrtEnv();
        using var session = TestData.CreateTwoInputSession(env);

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
        using var env = new OrtEnv();
        using var session = TestData.CreateTwoInputSession(env);

        session.Warmup(warmupRuns: 0, warmupBatchSize: -1);
    }

    [TestMethod]
    public void WarmupSupportsScalarInputs() => AssertIdentityWarmup([], [], 3);

    [TestMethod]
    [DataRow(1)]
    [DataRow(3)]
    public void WarmupPreservesFixedDimensions(int batchSize) =>
        AssertIdentityWarmup([2], [2], batchSize);

    [TestMethod]
    [DataRow(1)]
    [DataRow(3)]
    public void WarmupResolvesDynamicLeadingDimension(int batchSize) =>
        AssertIdentityWarmup([-1, 2], [batchSize, 2], batchSize);

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void DynamicBatchSizeMustBePositive(int batchSize)
    {
        using var env = new OrtEnv();
        using var session = new OrtSession(env, TestOnnxModels.CreateIdentity(-1, 2));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            session.Warmup(warmupBatchSize: batchSize));
        session.Warmup(warmupBatchSize: 2);
    }

    static void AssertIdentityWarmup(long[] modelDimensions, long[] inputDimensions, int batchSize)
    {
        using var env = new OrtEnv();
        using var session = new OrtSession(env, TestOnnxModels.CreateIdentity(modelDimensions));

        session.Warmup(warmupRuns: 2, warmupBatchSize: batchSize);

        var count = 1;
        foreach (var dimension in inputDimensions)
        {
            count *= checked((int)dimension);
        }
        var data = new float[count];
        Array.Fill(data, 7f);
        using var input = new OrtValue<float>(data, inputDimensions);
        using var output = new OrtValue<float>(new float[count], inputDimensions);
        session.Run(input, output);

        Assert.AreSequenceEqual(data, output.Data.ToArray());
        Assert.AreSequenceEqual(modelDimensions, session.Inputs.Span[0].Dimensions.ToArray());
    }
}
