using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OnnxRuntimeSharp.Test;

static class TestData
{
    public const string MnistModelFileName = "mnist-8.onnx";

    // Keep the resource stream alive for the lifetime of the returned spans.
    static readonly UnmanagedMemoryStream MnistModelStream =
        (UnmanagedMemoryStream)(typeof(TestData).Assembly.GetManifestResourceStream(MnistModelFileName)
            ?? throw new InvalidOperationException($"Embedded {MnistModelFileName} resource was not found."));

    public static unsafe bool HasPluginApi => Ort.Api->GetEpApi != null;

    public static void RequirePluginApi()
    {
        if (!HasPluginApi)
        {
            Assert.Inconclusive("Execution provider plugins require ONNX Runtime API 22 or later.");
        }
    }

    public static unsafe ReadOnlySpan<byte> GetMnistModelBytes() =>
        new(MnistModelStream.PositionPointer, checked((int)MnistModelStream.Length));

    public static OrtSession CreateMnistSession(OrtEnv env, OrtSessionOptions? options = null) =>
        new(env, GetMnistModelBytes(), options);

    public static OrtSession CreateTwoInputSession(
        OrtEnv env,
        OrtSessionOptions? options = null) =>
        new(env, TestOnnxModels.TwoInputTwoOutput, options);

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
