namespace OnnxRuntimeSharp;

public readonly unsafe struct OrtValueBinding
{
    internal OrtValueBinding(OrtSession session, OrtTensorInfo info, OrtSafeHandle<Ort.OrtValueHandle> value)
    {
        Session = session;
        Info = info;
        Value = value;
    }

    public OrtTensorInfo Info { get; }

    internal OrtSession Session { get; }

    internal sbyte* NamePointer => Info.NamePointer;

    internal Ort.OrtValueHandle ValueHandle => Value.Handle;

    internal OrtSafeHandle<Ort.OrtValueHandle> Value { get; }
}
