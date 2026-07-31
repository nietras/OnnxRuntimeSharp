using System;
using System.Collections.Generic;
using System.IO;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtSessionOptionsTest
{
    [TestMethod]
    public void ProductionInferenceOptionsCanBeConfigured()
    {
        using var options = new OrtSessionOptions();
        var optimizedModelPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.onnx");

        options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_ENABLE_ALL);
        options.SetExecutionMode(Ort.ExecutionMode.ORT_SEQUENTIAL);
        options.SetIntraOpThreadCount(1);
        options.SetInterOpThreadCount(1);
        options.SetMemoryPatternEnabled(true);
        options.SetCpuMemoryArenaEnabled(true);
        options.SetDeterministicCompute(true);
        options.SetLogId("test");
        options.SetLogSeverityLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR);
        options.SetLogVerbosityLevel(0);
        options.AddConfigEntry("session.intra_op.allow_spinning", "0");
        options.AddFreeDimensionOverrideByName("batch_size", 1);
        options.SetOptimizedModelFilePath(optimizedModelPath);
    }

    [TestMethod]
    public void ProfilingCanBeEnabledAndDisabled()
    {
        using var options = new OrtSessionOptions();

        options.EnableProfiling(Path.Combine(Path.GetTempPath(), $"ort-{Guid.NewGuid():N}"));
        options.DisableProfiling();
    }

    [TestMethod]
    public void GenericProviderOptionsAreAcceptedByAvailableCpuProvider()
    {
        using var options = new OrtSessionOptions();

        options.AppendExecutionProvider(
            "CPUExecutionProvider",
            new Dictionary<string, string>());
    }

    [TestMethod]
    public void EpSelectionPolicyCanBeConfigured()
    {
        using var options = new OrtSessionOptions();

        options.SetExecutionProviderSelectionPolicy(
            Ort.OrtExecutionProviderDevicePolicy.OrtExecutionProviderDevicePolicy_DEFAULT);
    }

    [TestMethod]
    public void InvalidThreadCountsAreRejected()
    {
        using var options = new OrtSessionOptions();

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => options.SetIntraOpThreadCount(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => options.SetInterOpThreadCount(0));
    }
}
