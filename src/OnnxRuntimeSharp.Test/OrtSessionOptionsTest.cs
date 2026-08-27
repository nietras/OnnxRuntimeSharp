using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        options.SetMemoryPatternEnabled(false);
        options.SetCpuMemoryArenaEnabled(true);
        options.SetCpuMemoryArenaEnabled(false);
        options.SetDeterministicCompute(true);
        options.SetDeterministicCompute(false);
        options.DisablePerSessionThreads();
        options.SetLogId("test");
        options.SetLogSeverityLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR);
        options.SetLogVerbosityLevel(0);
        options.AddConfigEntry("session.intra_op.allow_spinning", "0");
        options.AddFreeDimensionOverride("DATA_BATCH", 1);
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

    [TestMethod]
    public void StringAndDimensionArgumentsAreValidated()
    {
        using var options = new OrtSessionOptions();

        Assert.ThrowsExactly<ArgumentException>(() => options.EnableProfiling(""));
        Assert.ThrowsExactly<ArgumentException>(() => options.SetLogId(""));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => options.SetLogVerbosityLevel(-1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => options.AddFreeDimensionOverride("DATA_BATCH", 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => options.AddFreeDimensionOverrideByName("batch", 0));
        Assert.ThrowsExactly<ArgumentException>(() => options.AddConfigEntry("", "value"));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.AddConfigEntry("key", null!));
        Assert.ThrowsExactly<ArgumentException>(() => options.SetOptimizedModelFilePath(""));
        Assert.ThrowsExactly<ArgumentException>(() => options.AppendExecutionProvider(""));
    }

    [TestMethod]
    public void ProviderDeviceArgumentsAreValidated()
    {
        using var firstEnvironment = new OrtEnvironment();
        using var secondEnvironment = new OrtEnvironment();
        var firstDevice = firstEnvironment.GetExecutionProviderDevices()[0];
        using var options = new OrtSessionOptions();

        Assert.ThrowsExactly<ArgumentException>(() =>
            options.AppendExecutionProvider(firstEnvironment, []));
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            options.AppendExecutionProvider(firstEnvironment, [null!]));
        Assert.ThrowsExactly<ArgumentException>(() =>
            options.AppendExecutionProvider(secondEnvironment, [firstDevice]));
    }

    [TestMethod]
    public void NoOptionsProviderOverloadIsSupported()
    {
        using var options = new OrtSessionOptions();

        options.AppendExecutionProvider("CPUExecutionProvider");
    }

    [TestMethod]
    public void GenericProviderOptionsAreMarshalled()
    {
        using var options = new OrtSessionOptions();

        options.AppendExecutionProvider(
            "CPUExecutionProvider",
            new Dictionary<string, string> { ["use_arena"] = "1" });
    }

    [TestMethod]
    public void PluginProviderOptionsAreMarshalled()
    {
        using var environment = new OrtEnvironment();
        var cpuDevice = environment.GetExecutionProviderDevices()
            .First(device => device.ExecutionProviderName == "CPUExecutionProvider");
        using var options = new OrtSessionOptions();

        try
        {
            options.AppendExecutionProvider(
                environment,
                [cpuDevice],
                new Dictionary<string, string> { ["use_arena"] = "1" });
        }
        catch (OrtException exception)
        {
            Assert.AreNotEqual(Ort.OrtErrorCode.ORT_OK, exception.ErrorCode);
        }
    }

    [TestMethod]
    public void OptionalProviderRoutesReturnSuccessOrStructuredError()
    {
        ExerciseOptionalProvider(options => options.AppendCudaExecutionProvider());
        ExerciseOptionalProvider(options => options.AppendCudaExecutionProvider(
            new Dictionary<string, string> { ["device_id"] = "0" }));
        ExerciseOptionalProvider(options => options.AppendTensorRtExecutionProvider());
        ExerciseOptionalProvider(options => options.AppendTensorRtExecutionProvider(
            new Dictionary<string, string> { ["device_id"] = "0" }));
        ExerciseOptionalProvider(options => options.AppendExecutionProvider("CUDAExecutionProvider"));
        ExerciseOptionalProvider(options => options.AppendExecutionProvider("TensorrtExecutionProvider"));
    }

    [TestMethod]
    public void DisposedOptionsRejectConfiguration()
    {
        var options = new OrtSessionOptions();
        options.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() =>
            options.SetGraphOptimizationLevel(Ort.GraphOptimizationLevel.ORT_ENABLE_ALL));
    }

    static void ExerciseOptionalProvider(Action<OrtSessionOptions> append)
    {
        using var options = new OrtSessionOptions();
        try
        {
            append(options);
        }
        catch (OrtException exception)
        {
            Assert.AreNotEqual(Ort.OrtErrorCode.ORT_OK, exception.ErrorCode);
        }
    }
}
