using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public sealed unsafe class OrtIoBinding : OrtSafeHandle<Ort.OrtIoBindingHandle>
{
    readonly OrtSession _session;
    readonly List<SafeHandle> _boundInputs = [];
    readonly List<SafeHandle> _boundOutputs = [];
    bool _sessionReferenceAdded;

    internal OrtIoBinding(OrtSession session)
    {
        _session = session;
        try
        {
            session.DangerousAddRef(ref _sessionReferenceAdded);
            Ort.OrtIoBindingHandle binding;
            Ort.Ok(Ort.CreateIoBinding(session.Handle, &binding));
            SetHandle(binding.Value);
        }
        catch
        {
            ReleaseSessionReference();
            throw;
        }
    }

    public void BindInput<T>(int index, OrtTensor<T> value) where T : unmanaged =>
        BindInputValue(index, value);

    public void BindInput(int index, OrtValue value) =>
        BindInputValue(index, value);

    public void BindOutput<T>(int index, OrtTensor<T> value) where T : unmanaged =>
        BindOutputValue(index, value);

    public void BindOutput(int index, OrtValue value) =>
        BindOutputValue(index, value);

    public void BindOutputToDevice(int index, OrtMemoryInfo memoryInfo)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(memoryInfo);
        var info = GetInfo(_session.Outputs, index, nameof(index));
        AddBoundResource(
            _boundOutputs,
            memoryInfo,
            () => Ort.BindOutputToDevice(Handle, info.NamePointer, memoryInfo.Handle));
    }

    public void ClearInputs()
    {
        ThrowIfDisposed();
        Ort.ClearBoundInputs(Handle);
        ReleaseBoundValues(_boundInputs);
    }

    public void ClearOutputs()
    {
        ThrowIfDisposed();
        Ort.ClearBoundOutputs(Handle);
        ReleaseBoundValues(_boundOutputs);
    }

    public void SynchronizeInputs()
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SynchronizeBoundInputs(Handle));
    }

    public void SynchronizeOutputs()
    {
        ThrowIfDisposed();
        Ort.Ok(Ort.SynchronizeBoundOutputs(Handle));
    }

    public OrtValue[] GetOutputValues()
    {
        ThrowIfDisposed();
        Ort.OrtAllocator* allocator;
        Ort.Ok(Ort.GetAllocatorWithDefaultOptions(&allocator));
        Ort.OrtValueHandle* values;
        nuint valueCount;
        Ort.Ok(Ort.GetBoundOutputValues(Handle, allocator, &values, &valueCount));
        var result = new OrtValue[checked((int)valueCount)];
        var initializedCount = 0;
        try
        {
            for (var index = 0; index < result.Length; ++index)
            {
                var value = values[index];
                values[index] = default;
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
                if (!values[index].IsNull)
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

    internal OrtSession Session => _session;

    protected override bool ReleaseHandle()
    {
        ReleaseBoundValues(_boundOutputs);
        ReleaseBoundValues(_boundInputs);
        Ort.ReleaseIoBinding(Handle);
        ReleaseSessionReference();
        return true;
    }

    void BindInputValue(int index, OrtSafeHandle<Ort.OrtValueHandle> owner)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(owner);
        var info = GetInfo(_session.Inputs, index, nameof(index));
        AddBoundValue(
            _boundInputs,
            owner,
            value => Ort.BindInput(Handle, info.NamePointer, value));
    }

    void BindOutputValue(int index, OrtSafeHandle<Ort.OrtValueHandle> owner)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(owner);
        var info = GetInfo(_session.Outputs, index, nameof(index));
        AddBoundValue(
            _boundOutputs,
            owner,
            value => Ort.BindOutput(Handle, info.NamePointer, value));
    }

    static OrtTensorInfo GetInfo(IReadOnlyList<OrtTensorInfo> infos, int index, string parameterName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index, parameterName);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, infos.Count, parameterName);
        return infos[index];
    }

    static void AddBoundValue(List<SafeHandle> values, OrtSafeHandle<Ort.OrtValueHandle> value, BindAction bind)
    {
        var referenceAdded = false;
        try
        {
            value.DangerousAddRef(ref referenceAdded);
            Ort.Ok(bind(value.Handle));
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

    void ReleaseSessionReference()
    {
        if (!_sessionReferenceAdded)
        {
            return;
        }

        _session.DangerousRelease();
        _sessionReferenceAdded = false;
    }

    delegate Ort.OrtStatusHandle BindAction(Ort.OrtValueHandle value);
    delegate Ort.OrtStatusHandle BindResourceAction();
}
