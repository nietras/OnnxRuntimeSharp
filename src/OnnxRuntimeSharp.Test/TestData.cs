using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

static class TestData
{
    public static string MnistModelPath => Path.Combine(AppContext.BaseDirectory, "mnist-8.onnx");

    public static byte[] ReadMnistModel() => File.ReadAllBytes(MnistModelPath);

    public static OrtSession CreateMnistSession(OrtEnvironment environment, OrtSessionOptions? options = null) =>
        new(environment, ReadMnistModel(), options);

    public static OrtTensor<float> CreateMnistInput() =>
        new(new float[28 * 28], [1, 1, 28, 28]);

    public static OrtTensor<float> CreateMnistOutput() =>
        new(new float[10], [1, 10]);

    public static IReadOnlyList<string> AvailableExecutionProviders { get; } =
        Ort.GetAvailableExecutionProviders();

    public static IEnumerable<string> AvailableAcceleratedExecutionProviders =>
        AvailableExecutionProviders.Where(name =>
            !string.Equals(name, "CPUExecutionProvider", StringComparison.Ordinal));
}
