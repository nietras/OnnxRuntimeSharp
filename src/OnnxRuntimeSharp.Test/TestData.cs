using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

static class TestData
{
    public static unsafe bool HasPluginApi => Ort.Api->GetEpApi != null;

    public static void RequirePluginApi()
    {
        if (!HasPluginApi)
        {
            Assert.Inconclusive("Execution provider plugins require ONNX Runtime API 22 or later.");
        }
    }

    public static string MnistModelPath => Path.Combine(AppContext.BaseDirectory, "mnist-8.onnx");

    public static byte[] ReadMnistModel() => File.ReadAllBytes(MnistModelPath);

    public static OrtSession CreateMnistSession(OrtEnv environment, OrtSessionOptions? options = null) =>
        new(environment, ReadMnistModel(), options);

    public static OrtSession CreateTwoInputSession(
        OrtEnv environment,
        OrtSessionOptions? options = null) =>
        new(environment, TestOnnxModels.TwoInputTwoOutput, options);

    public static OrtValue<float> CreateMnistInput() =>
        new(new float[28 * 28], [1, 1, 28, 28]);

    public static OrtValue<float> CreateMnistOutput() =>
        new(new float[10], [1, 10]);

    public static IReadOnlyList<string> AvailableExecutionProviders { get; } =
        Ort.GetAvailableExecutionProviders();

    public static IEnumerable<string> AvailableAcceleratedExecutionProviders =>
        AvailableExecutionProviders.Where(name =>
            !string.Equals(name, "CPUExecutionProvider", StringComparison.Ordinal));
}
