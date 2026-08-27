using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtEpDevice
{
    internal OrtEpDevice(OrtEnvironment environment, Ort.OrtEpDevice* pointer)
    {
        Environment = environment;
        Pointer = pointer;
        ExecutionProviderName = ReadString(Ort.EpDevice_EpName(pointer));
        ExecutionProviderVendor = ReadString(Ort.EpDevice_EpVendor(pointer));
        ExecutionProviderMetadata = ReadKeyValuePairs(Ort.EpDevice_EpMetadata(pointer));
        ExecutionProviderOptions = ReadKeyValuePairs(Ort.EpDevice_EpOptions(pointer));

        var hardwareDevice = Ort.EpDevice_Device(pointer);
        HardwareDevice = new OrtHardwareDeviceInfo(
            Ort.HardwareDevice_Type(hardwareDevice),
            Ort.HardwareDevice_VendorId(hardwareDevice),
            ReadString(Ort.HardwareDevice_Vendor(hardwareDevice)),
            Ort.HardwareDevice_DeviceId(hardwareDevice),
            ReadKeyValuePairs(Ort.HardwareDevice_Metadata(hardwareDevice)));
    }

    public string ExecutionProviderName { get; }
    public string ExecutionProviderVendor { get; }
    public IReadOnlyDictionary<string, string> ExecutionProviderMetadata { get; }
    public IReadOnlyDictionary<string, string> ExecutionProviderOptions { get; }
    public OrtHardwareDeviceInfo HardwareDevice { get; }

    internal OrtEnvironment Environment { get; }
    internal Ort.OrtEpDevice* Pointer { get; }

    static string ReadString(sbyte* value) =>
        Marshal.PtrToStringUTF8((IntPtr)value) ?? string.Empty;

    static Dictionary<string, string> ReadKeyValuePairs(Ort.OrtKeyValuePairs* pairs)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        if (pairs is null)
        {
            return result;
        }

        sbyte** keys;
        sbyte** values;
        nuint count;
        Ort.GetKeyValuePairs(pairs, &keys, &values, &count);
        for (nuint index = 0; index < count; ++index)
        {
            result[ReadString(keys[index])] = ReadString(values[index]);
        }
        return result;
    }
}

public sealed record OrtHardwareDeviceInfo(
    Ort.OrtHardwareDeviceType Type,
    uint VendorId,
    string Vendor,
    uint DeviceId,
    IReadOnlyDictionary<string, string> Metadata);
