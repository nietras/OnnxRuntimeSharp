using System;
using System.Collections.Generic;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtSessionFactoryTest
{
    [TestMethod]
    public void CpuSingleThreadFactoryReturnsUsableWarmedSession()
    {
        using var session = OrtSessionFactory.CreateOnCPUSingleThread(TestOnnxModels.TwoInputTwoOutput);

        AssertIdentityInference(session);
        Assert.IsFalse(OrtEnv.Instance().IsClosed);
    }

    [TestMethod]
    public void FactorySelectsFirstAvailableConfiguration()
    {
        var appended = new List<string>();
        var environments = new List<OrtEnv>();
        var unavailable = new OrtEpConfig("Unavailable", _ =>
        {
            appended.Add("Unavailable");
            throw new NotSupportedException("Unavailable test provider.");
        });
        var first = new OrtEpConfig("First", (env, _) =>
        {
            appended.Add("First");
            environments.Add(env);
        }, allowsCpuFallback: true);
        var second = new OrtEpConfig("Second", _ => appended.Add("Second"), allowsCpuFallback: true);

        using var session = OrtSessionFactory.Create(
            TestOnnxModels.TwoInputTwoOutput, [unavailable, first, second]);

        Assert.AreSequenceEqual(["Unavailable", "First", "Second", "First"], appended);
        Assert.HasCount(2, environments);
        Assert.AreSame(OrtEnv.Instance(), environments[0]);
        Assert.AreSame(environments[0], environments[1]);
        AssertIdentityInference(session);
    }

    [TestMethod]
    public void EmptyPriorityUsesDefaultCpuAndCanSkipWarmup()
    {
        using var session = OrtSessionFactory.Create(
            TestOnnxModels.TwoInputTwoOutput, [], warmupRuns: 0, warmupBatchSize: -1);

        AssertIdentityInference(session);
    }

    static void AssertIdentityInference(OrtSession session)
    {
        using var first = new OrtValue<float>([3], [1]);
        using var second = new OrtValue<float>([7], [1]);
        using var firstOutput = new OrtValue<float>(new float[1], [1]);
        using var secondOutput = new OrtValue<float>(new float[1], [1]);
        session.Run(
            [session.CreateInputBinding(0, first), session.CreateInputBinding(1, second)],
            [session.CreateOutputBinding(0, firstOutput), session.CreateOutputBinding(1, secondOutput)]);

        Assert.AreEqual(3f, firstOutput.Data[0]);
        Assert.AreEqual(7f, secondOutput.Data[0]);
    }
}
