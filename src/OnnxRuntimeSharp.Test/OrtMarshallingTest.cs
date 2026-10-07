using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtMarshallingTest
{
    [TestMethod]
    public unsafe void KeyValuePairsRoundTripAndDispose()
    {
        var options = new Dictionary<string, string>
        {
            ["first"] = "value-\u00e6-\u03bb-\u65e5\u672c",
            ["second"] = "",
        };
        var keys = stackalloc sbyte*[2];
        var values = stackalloc sbyte*[2];
        var pairs = new OrtUtf8KeyValuePairs(options, keys, values, 2);
        try
        {
            Assert.AreEqual((nuint)2, pairs.Count);
            for (var index = 0; index < 2; ++index)
            {
                var key = Marshal.PtrToStringUTF8((IntPtr)pairs.Keys[index])!;
                var value = Marshal.PtrToStringUTF8((IntPtr)pairs.Values[index]);
                Assert.AreEqual(options[key], value);
            }
        }
        finally
        {
            pairs.Dispose();
        }

        Assert.AreEqual((nuint)0, pairs.Count);
        Assert.IsTrue(keys[0] is null && keys[1] is null);
        Assert.IsTrue(values[0] is null && values[1] is null);
        pairs.Dispose();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public unsafe void NullAndEmptyOptionsAreSupported(bool emptyDictionary)
    {
        var options = emptyDictionary ? new Dictionary<string, string>() : null;
        using var pairs = new OrtUtf8KeyValuePairs(options, null, null, 0);

        Assert.AreEqual((nuint)0, pairs.Count);
        Assert.IsTrue(pairs.Keys is null);
        Assert.IsTrue(pairs.Values is null);
    }

    [TestMethod]
    public unsafe void PartialPairIsFreedWhenValueIsInvalid()
    {
        var options = new Dictionary<string, string>
        {
            ["first"] = "value",
            ["second"] = null!,
        };
        var keys = stackalloc sbyte*[2];
        var values = stackalloc sbyte*[2];

        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            using var pairs = new OrtUtf8KeyValuePairs(options, keys, values, 2);
        });

        Assert.IsTrue(keys[0] is null && keys[1] is null);
        Assert.IsTrue(values[0] is null && values[1] is null);
    }

    [TestMethod]
    public unsafe void InsufficientCapacityFreesConvertedPairs()
    {
        var options = new Dictionary<string, string>
        {
            ["first"] = "value",
            ["second"] = "value",
        };
        var keys = stackalloc sbyte*[1];
        var values = stackalloc sbyte*[1];

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
        {
            using var pairs = new OrtUtf8KeyValuePairs(options, keys, values, 1);
        });

        Assert.IsTrue(keys[0] is null);
        Assert.IsTrue(values[0] is null);
    }

    [TestMethod]
    public unsafe void EnumerationFailureFreesConvertedPairs()
    {
        var keys = stackalloc sbyte*[2];
        var values = stackalloc sbyte*[2];

        Assert.ThrowsExactly<InvalidOperationException>(() =>
        {
            using var pairs = new OrtUtf8KeyValuePairs(new FailingOptions(), keys, values, 2);
        });

        Assert.IsTrue(keys[0] is null);
        Assert.IsTrue(values[0] is null);
    }

    [TestMethod]
    [DataRow("model.onnx")]
    [DataRow("model-\u00e6-\u03bb-\u65e5\u672c.onnx")]
    public unsafe void NativePathUsesPlatformEncoding(string path)
    {
        fixed (char* pointer = path)
        {
            var nativePath = new OrtNativePath(path, pointer);
            try
            {
                var actual = OperatingSystem.IsWindows()
                    ? Marshal.PtrToStringUni((IntPtr)nativePath.Pointer)
                    : Marshal.PtrToStringUTF8((IntPtr)nativePath.Pointer);
                Assert.AreEqual(path, actual);
                if (OperatingSystem.IsWindows())
                {
                    Assert.AreEqual((IntPtr)pointer, (IntPtr)nativePath.Pointer);
                }
            }
            finally
            {
                nativePath.Dispose();
            }

            Assert.IsTrue(nativePath.Pointer is null);
            nativePath.Dispose();
        }
    }

    [TestMethod]
    public unsafe void NativePathScopeDoesNotAllocateManagedMemory()
    {
        const string path = "model.onnx";
        fixed (char* pointer = path)
        {
            for (var warmup = 0; warmup < 3; ++warmup)
            {
                using var nativePath = new OrtNativePath(path, pointer);
            }

            var allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();
            for (var iteration = 0; iteration < 1_000; ++iteration)
            {
                using var nativePath = new OrtNativePath(path, pointer);
            }
            Assert.AreEqual(0, GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore);
        }
    }

    sealed class FailingOptions : IReadOnlyDictionary<string, string>
    {
        public int Count => 2;
        public string this[string key] => throw new NotSupportedException();
        public IEnumerable<string> Keys => throw new NotSupportedException();
        public IEnumerable<string> Values => throw new NotSupportedException();
        public bool ContainsKey(string key) => throw new NotSupportedException();
        public bool TryGetValue(string key, out string value) => throw new NotSupportedException();

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            yield return new KeyValuePair<string, string>("first", "value");
            throw new InvalidOperationException("Enumeration failed.");
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
