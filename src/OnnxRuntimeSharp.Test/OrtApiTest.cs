using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public unsafe class OrtApiTest
{
    [TestMethod]
    [DataRow("1.21.0", 21u)]
    [DataRow("1.21.0-dev", 21u)]
    [DataRow("1.9.1", 9u)]
    [DataRow("1.21", 21u)]
    [DataRow("1.28.0", Ort.MaxApiVersion)]
    [DataRow("1.30.0", Ort.MaxApiVersion)]
    [DataRow("1.999.0", Ort.MaxApiVersion)]
    [DataRow("1.4294967295.0", Ort.MaxApiVersion)]
    [DataRow("1.4294967296.0", Ort.MaxApiVersion)]
    [DataRow("1.0.0", Ort.MaxApiVersion)]
    [DataRow("2.21.0", Ort.MaxApiVersion)]
    [DataRow("1.-21.0", Ort.MaxApiVersion)]
    [DataRow("1.+21.0", Ort.MaxApiVersion)]
    [DataRow("1. 21.0", Ort.MaxApiVersion)]
    [DataRow("1..0", Ort.MaxApiVersion)]
    [DataRow("unknown", Ort.MaxApiVersion)]
    [DataRow("", Ort.MaxApiVersion)]
    [DataRow(null, Ort.MaxApiVersion)]
    public void VersionHintParsesMinorOrFallsBack(string? runtimeVersion, uint expected)
    {
        var bytes = runtimeVersion is null ? ReadOnlySpan<byte>.Empty : Encoding.UTF8.GetBytes(runtimeVersion);
        Assert.AreEqual(expected, Ort.GetApiVersionHint(bytes));
    }

    [TestMethod]
    [DataRow(1u, nameof(Ort.OrtApi.ReleaseCustomOpDomain))]
    [DataRow(2u, nameof(Ort.OrtApi.ReleaseModelMetadata))]
    [DataRow(3u, nameof(Ort.OrtApi.AddFreeDimensionOverrideByName))]
    [DataRow(4u, nameof(Ort.OrtApi.ReleaseAvailableProviders))]
    [DataRow(5u, nameof(Ort.OrtApi.SetGlobalSpinControl))]
    [DataRow(6u, nameof(Ort.OrtApi.ReleaseArenaCfg))]
    [DataRow(7u, nameof(Ort.OrtApi.GetCurrentGpuDeviceId))]
    [DataRow(8u, nameof(Ort.OrtApi.CreateSessionFromArrayWithPrepackedWeightsContainer))]
    [DataRow(9u, nameof(Ort.OrtApi.GetSparseTensorIndices))]
    [DataRow(10u, nameof(Ort.OrtApi.SynchronizeBoundOutputs))]
    [DataRow(11u, nameof(Ort.OrtApi.SessionOptionsAppendExecutionProvider_MIGraphX))]
    [DataRow(12u, nameof(Ort.OrtApi.ReleaseKernelInfo))]
    [DataRow(13u, nameof(Ort.OrtApi.ReleaseCANNProviderOptions))]
    [DataRow(14u, nameof(Ort.OrtApi.GetSessionConfigEntry))]
    [DataRow(15u, nameof(Ort.OrtApi.GetBuildInfoString))]
    [DataRow(16u, nameof(Ort.OrtApi.KernelContext_GetResource))]
    [DataRow(17u, nameof(Ort.OrtApi.SessionOptionsAppendExecutionProvider_OpenVINO_V2))]
    [DataRow(18u, nameof(Ort.OrtApi.AddExternalInitializersFromFilesInMemory))]
    [DataRow(19u, nameof(Ort.OrtApi.AddExternalInitializersFromFilesInMemory))]
    [DataRow(20u, nameof(Ort.OrtApi.SetEpDynamicOptions))]
    [DataRow(21u, nameof(Ort.OrtApi.SetEpDynamicOptions))]
    [DataRow(22u, nameof(Ort.OrtApi.GetEpApi))]
    [DataRow(23u, nameof(Ort.OrtApi.CreateExternalInitializerInfo))]
    [DataRow(24u, nameof(Ort.OrtApi.GetTensorElementTypeAndShapeDataReference))]
    [DataRow(25u, nameof(Ort.OrtApi.SetPerSessionThreadPoolCallbacks))]
    [DataRow(26u, nameof(Ort.OrtApi.SetPerSessionThreadPoolCallbacks))]
    [DataRow(27u, nameof(Ort.OrtApi.SessionReleaseCapturedGraph))]
    [DataRow(Ort.MaxApiVersion, nameof(Ort.OrtApi.KernelContext_GetSyncStream))]
    public void CopyPreservesSupportedPrefixAndZerosTail(uint version, string lastField)
    {
        var pointerCount = PointerCountThrough(lastField);
        var fullPointerCount = sizeof(Ort.OrtApi) / sizeof(nint);
        var source = stackalloc nint[fullPointerCount];
        for (var index = 0; index < fullPointerCount; ++index)
        {
            source[index] = index + 1;
        }

        var copy = (nint*)Ort.CopyApi((Ort.OrtApi*)source, version);
        for (var index = 0; index < fullPointerCount; ++index)
        {
            Assert.AreEqual(index < pointerCount ? (nint)(index + 1) : 0, copy[index]);
            Assert.AreEqual((nint)(index + 1), source[index]);
        }
    }

    [TestMethod]
    public void LayoutMatchesApi28()
    {
        Assert.AreEqual(sizeof(Ort.OrtApi), PointerCountThrough(nameof(Ort.OrtApi.KernelContext_GetSyncStream)) * sizeof(nint));
    }

    [TestMethod]
    public void LoadedApiSupportsCoreFunctions()
    {
        Assert.IsTrue(Ort.Api->CreateEnv != null);
        Assert.IsTrue(Ort.Api->Run != null);
        Assert.IsTrue(Ort.Api->ReleaseValue != null);
        CollectionAssert.Contains(Ort.GetAvailableExecutionProviders(), "CPUExecutionProvider");
    }

    [TestMethod]
    public void LoadedApiMatchesNativeRuntime()
    {
        var apiBase = Ort.NativeExports.OrtGetApiBase();
        var runtimeVersion = Marshal.PtrToStringUTF8((nint)apiBase->GetVersionString());
        Console.WriteLine($"Runtime: {runtimeVersion}; Architecture: {RuntimeInformation.ProcessArchitecture}");
        var version = Ort.ApiVersion;
        Assert.IsTrue(version > 0 && version <= Ort.MaxApiVersion);
        var native = apiBase->GetApi(version);
        Assert.IsTrue(native != null);
        if (version == Ort.MaxApiVersion)
        {
            Assert.AreEqual((nint)native, (nint)Ort.Api);
        }
        else
        {
            Assert.AreNotEqual((nint)native, (nint)Ort.Api);
        }
        Assert.AreEqual((nint)native->CreateEnv, (nint)Ort.Api->CreateEnv);
        Assert.AreEqual((nint)native->Run, (nint)Ort.Api->Run);
        if (OperatingSystem.IsWindows() && IntPtr.Size == 4)
        {
            Assert.AreEqual("1.21.0", runtimeVersion);
            Assert.AreEqual(21u, version);
            Assert.IsTrue(Ort.Api->SetEpDynamicOptions != null);
            var table = (nint*)Ort.Api;
            var pointerCount = PointerCountThrough(nameof(Ort.OrtApi.SetEpDynamicOptions));
            var fullPointerCount = sizeof(Ort.OrtApi) / sizeof(nint);
            for (var index = pointerCount; index < fullPointerCount; ++index)
            {
                Assert.AreEqual((nint)0, table[index]);
            }
        }
    }

    static int PointerCountThrough(string fieldName) =>
        (int)Marshal.OffsetOf<Ort.OrtApi>(fieldName) / sizeof(nint) + 1;
}
