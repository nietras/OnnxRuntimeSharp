using System;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtSafeHandleTest
{
    [TestMethod]
    public void NativeHandlesPreservePointersAndShareLifetimeBehavior()
    {
        AssertHandle<Ort.OrtEnvHandle>();
        AssertHandle<Ort.OrtValueHandle>();
        AssertHandle<Ort.OrtMemoryInfoHandle>();
        AssertHandle<Ort.OrtIoBindingHandle>();
        AssertHandle<Ort.OrtSessionHandle>();
        AssertHandle<Ort.OrtRunOptionsHandle>();
        AssertHandle<Ort.OrtSessionOptionsHandle>();
    }

    [TestMethod]
    public unsafe void NativeSessionOptionsCanBeClonedWithTypedHandles()
    {
        using var options = new OrtSessionOptions();
        Ort.OrtSessionOptionsHandle clone;
        Ort.CloneSessionOptions(options.Handle, &clone).Ok();
        try
        {
            Assert.IsFalse(clone.IsNull);
            Assert.AreNotEqual(options.Handle, clone);
            Ort.SetIntraOpNumThreads(clone, 1).Ok();
        }
        finally
        {
            Ort.ReleaseSessionOptions(clone);
        }
    }

    [TestMethod]
    public unsafe void AllocatorReturnsTypedMemoryInfoHandles()
    {
        Ort.OrtAllocator* allocator;
        Ort.GetAllocatorWithDefaultOptions(&allocator).Ok();
        Ort.OrtMemoryInfoHandle info;
        Ort.AllocatorGetInfo(allocator, &info).Ok();

        Assert.IsFalse(info.IsNull);
        Assert.AreEqual(info, allocator->Info(allocator));

        using var tensor = new OrtTensor<float>([1], [1]);
        Ort.OrtMemoryInfoHandle tensorInfo;
        Ort.GetTensorMemoryInfo(tensor.Handle, &tensorInfo).Ok();
        Assert.IsFalse(tensorInfo.IsNull);
        int deviceId;
        Ort.MemoryInfoGetId(tensorInfo, &deviceId).Ok();
        Assert.AreEqual(0, deviceId);
        int comparison;
        Ort.CompareMemoryInfo(info, allocator->Info(allocator), &comparison).Ok();
        Assert.AreEqual(0, comparison);
    }

    [TestMethod]
    [DataRow(typeof(OrtEnv), typeof(OrtSafeHandle<Ort.OrtEnvHandle>))]
    [DataRow(typeof(OrtValue), typeof(OrtSafeHandle<Ort.OrtValueHandle>))]
    [DataRow(typeof(OrtTensor<float>), typeof(OrtSafeHandle<Ort.OrtValueHandle>))]
    [DataRow(typeof(OrtMemoryInfo), typeof(OrtSafeHandle<Ort.OrtMemoryInfoHandle>))]
    [DataRow(typeof(OrtIoBinding), typeof(OrtSafeHandle<Ort.OrtIoBindingHandle>))]
    [DataRow(typeof(OrtSession), typeof(OrtSafeHandle<Ort.OrtSessionHandle>))]
    [DataRow(typeof(OrtRunOptions), typeof(OrtSafeHandle<Ort.OrtRunOptionsHandle>))]
    [DataRow(typeof(OrtSessionOptions), typeof(OrtSafeHandle<Ort.OrtSessionOptionsHandle>))]
    public void WrappersUseTypedSafeHandle(Type wrapperType, Type expectedBaseType)
    {
        Assert.AreEqual(expectedBaseType, wrapperType.BaseType);
    }

    static unsafe void AssertHandle<THandle>()
        where THandle : unmanaged, Ort.IOrtHandle<THandle>
    {
        Assert.AreEqual(IntPtr.Size, sizeof(THandle));
        using var invalid = new TestSafeHandle<THandle>(IntPtr.Zero);
        Assert.IsTrue(invalid.IsInvalid);
        Assert.IsTrue(invalid.Handle.IsNull);
        Assert.ThrowsExactly<ObjectDisposedException>(() => invalid.CheckDisposed());

        var pointer = new IntPtr(42);
        using var valid = new TestSafeHandle<THandle>(pointer);
        Assert.IsFalse(valid.IsInvalid);
        Assert.AreEqual(pointer, valid.Handle.Value);
        Assert.IsFalse(valid.Handle.IsNull);
        valid.CheckDisposed();

        valid.Dispose();
        Assert.IsTrue(valid.IsClosed);
        Assert.ThrowsExactly<ObjectDisposedException>(() => valid.CheckDisposed());
        Assert.AreEqual(1, valid.ReleaseCount);
        valid.Dispose();
        Assert.AreEqual(1, valid.ReleaseCount);
    }

    sealed class TestSafeHandle<THandle> : OrtSafeHandle<THandle>
        where THandle : unmanaged, Ort.IOrtHandle<THandle>
    {
        public TestSafeHandle(IntPtr pointer) => SetHandle(pointer);

        public int ReleaseCount { get; private set; }

        public void CheckDisposed() => ThrowIfDisposed();

        protected override bool ReleaseHandle()
        {
            ++ReleaseCount;
            return true;
        }
    }
}
