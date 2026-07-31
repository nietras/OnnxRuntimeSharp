using System;
using System.IO;
using Xunit;

namespace OnnxRuntimeSharp.Test;

public class OrtSessionTest
{
    [Fact]
    public void MnistInferenceProducesFiniteScores()
    {
        using var environment = new OrtEnvironment();
        using var session = new OrtSession(environment, File.ReadAllBytes(GetModelPath()));
        using var input = new OrtTensor<float>(new float[28 * 28], [1, 1, 28, 28]);
        using var output = new OrtTensor<float>(new float[10], [1, 10]);

        session.Run(input, output);

        Assert.All(output.Data.ToArray(), value => Assert.True(float.IsFinite(value)));
    }

    [Fact]
    public void SessionProvidesModelTensorDimensions()
    {
        using var environment = new OrtEnvironment();
        using var session = new OrtSession(environment, File.ReadAllBytes(GetModelPath()));

        Assert.Equal([1, 1, 28, 28], session.InputDimensions.ToArray());
        Assert.Equal([1, 10], session.OutputDimensions.ToArray());
    }

    [Fact]
    public void CachedTensorsRunWithoutManagedAllocations()
    {
        using var environment = new OrtEnvironment();
        using var session = new OrtSession(environment, File.ReadAllBytes(GetModelPath()));
        using var input = new OrtTensor<float>(new float[28 * 28], [1, 1, 28, 28]);
        using var output = new OrtTensor<float>(new float[10], [1, 10]);

        for (var warmup = 0; warmup < 3; ++warmup)
        {
            input.Data[0] = warmup;
            session.Run(input, output);
        }

        var allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();
        for (var iteration = 0; iteration < 10_000; ++iteration)
        {
            input.Data[0] = iteration;
            session.Run(input, output);
        }

        Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore);
        Assert.All(output.Data.ToArray(), value => Assert.True(float.IsFinite(value)));
    }

    [Fact]
    public void NamedBindingsRunWithoutManagedAllocations()
    {
        using var environment = new OrtEnvironment();
        using var session = new OrtSession(environment, File.ReadAllBytes(GetModelPath()));
        using var input = new OrtTensor<float>(new float[28 * 28], [1, 1, 28, 28]);
        using var output = new OrtTensor<float>(new float[10], [1, 10]);
        var inputs = new[] { session.CreateInputBinding(0, input) };
        var outputs = new[] { session.CreateOutputBinding(0, output) };

        for (var warmup = 0; warmup < 3; ++warmup)
        {
            session.Run(inputs, outputs);
        }

        var allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();
        for (var iteration = 0; iteration < 10_000; ++iteration)
        {
            input.Data[0] = iteration;
            session.Run(inputs, outputs);
        }

        Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore);
        Assert.All(output.Data.ToArray(), value => Assert.True(float.IsFinite(value)));
    }

    [Fact]
    public void ProfilingReturnsReportPath()
    {
        var profilePrefix = Path.Combine(Path.GetTempPath(), $"OnnxRuntimeSharp-{Guid.NewGuid():N}");
        using var environment = new OrtEnvironment();
        using var options = new OrtSessionOptions();
        options.EnableProfiling(profilePrefix);
        using var session = new OrtSession(environment, File.ReadAllBytes(GetModelPath()), options);
        using var input = new OrtTensor<float>(new float[28 * 28], [1, 1, 28, 28]);
        using var output = new OrtTensor<float>(new float[10], [1, 10]);

        session.Run(input, output);
        var profilePath = session.EndProfiling();

        try
        {
            Assert.True(File.Exists(profilePath), $"Missing profiling report '{profilePath}'.");
        }
        finally
        {
            File.Delete(profilePath);
        }
    }

    static string GetModelPath() => Path.Combine(AppContext.BaseDirectory, "mnist-8.onnx");
}
