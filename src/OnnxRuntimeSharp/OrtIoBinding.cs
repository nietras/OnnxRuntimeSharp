using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtIoBinding : SafeHandle
{
    readonly OrtSession _session;
    readonly List<SafeHandle> _boundInputs = [];
    readonly List<SafeHandle> _boundOutputs = [];
    bool _sessionReferenceAdded;

    internal OrtIoBinding(OrtSession session)
        : base(IntPtr.Zero, ownsHandle: true)
    {
        _session = session;
        try
        {
            session.DangerousAddRef(ref _sessionReferenceAdded);
            Ort.OrtIoBinding* binding;
            Ort.Ok(Ort.CreateIoBinding(session.Pointer, &binding));
            SetHandle((IntPtr)binding);
        }
        catch
        {
            ReleaseSessionReference();
            throw;
        }
    }

    public void BindInput<T>(int index, OrtTensor<T> value) where T : unmanaged =>
        BindInput(index, (SafeHandle)value);

    public void BindInput(int index, OrtValue value) =>
        BindInput(index, (SafeHandle)value);

    public void BindOutput<T>(int index, OrtTensor<T> value) where T : unmanaged =>
        BindOutput(index, (SafeHandle)value);

    public void BindOutput(int index, OrtValue value) =>
        BindOutput(index, (SafeHandle)value);

    public void BindOutputToDevice(int index, OrtMemoryInfo memoryInfo)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(memoryInfo);
        var info = GetInfo(_session.Outputs, index, nameof(index));
        AddBoundResource(
            _boundOutputs,
            memoryInfo,
            () => Ort.BindOutputToDevice(Pointer, info.NamePointer, memoryInfo.Pointer));
    }

    public void ClearInputs()
    {
        ThrowIfDisposed();
        Ort.ClearBoundInputs(Pointer);
        ReleaseBoundValues(_boundInputs);
    }

    public void ClearOutputs()
    {
        ThrowIfDisposed();
        Ort.ClearBoundOutputs(Pointer);
        ReleaseBoundValues(_boundOutputs);
    }

    public void SynchronizeInputs()
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SynchronizeBoundInputs(Pointer));
    }

    public void SynchronizeOutputs()
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SynchronizeBoundOutputs(Pointer));
    }

    public OrtValue[] GetOutputValues()
    {
        ThrowIfDisposed();
        Ort.OrtAllocator* allocator;
        Ort.Ok(Ort.GetAllocatorWithDefaultOptions(&allocator));
        Ort.OrtValue** values;
        nuint valueCount;
        Ort.Ok(Ort.GetBoundOutputValues(Pointer, allocator, &values, &valueCount));
        var result = new OrtValue[checked((int)valueCount)];
        var initializedCount = 0;
        try
        {
            for (var index = 0; index < result.Length; ++index)
            {
                var value = values[index];
                values[index] = null;
                result[index] = new OrtValue(value);
                ++initializedCount;
            }
            return result;
        }
        catch
        {
            for (var index = 0; index < initializedCount; ++index)
            {
                result[index].Dispose();
            }
            for (var index = initializedCount; index < result.Length; ++index)
            {
                if (values[index] is not null)
                {
                    Ort.ReleaseValue(values[index]);
                }
            }
            throw;
        }
        finally
        {
            Ort.ReleaseAllocatorValue(allocator, values);
        }
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    internal OrtSession Session => _session;
    internal Ort.OrtIoBinding* Pointer => (Ort.OrtIoBinding*)handle;

    protected override bool ReleaseHandle()
    {
        ReleaseBoundValues(_boundOutputs);
        ReleaseBoundValues(_boundInputs);
        Ort.ReleaseIoBinding(Pointer);
        ReleaseSessionReference();
        return true;
    }

    void BindInput(int index, SafeHandle owner)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(owner);
        var info = GetInfo(_session.Inputs, index, nameof(index));
        AddBoundValue(
            _boundInputs,
            owner,
            value => Ort.BindInput(Pointer, info.NamePointer, value));
    }

    void BindOutput(int index, SafeHandle owner)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(owner);
        var info = GetInfo(_session.Outputs, index, nameof(index));
        AddBoundValue(
            _boundOutputs,
            owner,
            value => Ort.BindOutput(Pointer, info.NamePointer, value));
    }

    static OrtTensorInfo GetInfo(IReadOnlyList<OrtTensorInfo> infos, int index, string parameterName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index, parameterName);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, infos.Count, parameterName);
        return infos[index];
    }

    static void AddBoundValue(List<SafeHandle> values, SafeHandle value, BindAction bind)
    {
        var referenceAdded = false;
        try
        {
            value.DangerousAddRef(ref referenceAdded);
            Ort.Ok(bind((Ort.OrtValue*)value.DangerousGetHandle()));
            values.Add(value);
            referenceAdded = false;
        }
        finally
        {
            if (referenceAdded)
            {
                value.DangerousRelease();
            }
        }
    }

    static void AddBoundResource(List<SafeHandle> values, SafeHandle value, BindResourceAction bind)
    {
        var referenceAdded = false;
        try
        {
            value.DangerousAddRef(ref referenceAdded);
            Ort.Ok(bind());
            values.Add(value);
            referenceAdded = false;
        }
        finally
        {
            if (referenceAdded)
            {
                value.DangerousRelease();
            }
        }
    }

    static void ReleaseBoundValues(List<SafeHandle> values)
    {
        foreach (var value in values)
        {
            value.DangerousRelease();
        }
        values.Clear();
    }

    void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(IsClosed || IsInvalid, this);

    void ReleaseSessionReference()
    {
        if (!_sessionReferenceAdded)
        {
            return;
        }

        _session.DangerousRelease();
        _sessionReferenceAdded = false;
    }

    delegate Ort.OrtStatusHandle BindAction(Ort.OrtValue* value);
    delegate Ort.OrtStatusHandle BindResourceAction();
}
