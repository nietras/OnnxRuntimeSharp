using System.Runtime.InteropServices;

namespace OnnxRuntimeSharp;

public readonly unsafe struct OrtValueBinding
{
    internal OrtValueBinding(OrtSession session, OrtTensorInfo info, SafeHandle value)
    {
        Session = session;
        Info = info;
        Value = value;
    }

    public OrtTensorInfo Info { get; }

    internal OrtSession Session { get; }

    internal sbyte* NamePointer => Info.NamePointer;

    internal Ort.OrtValueHandle ValueHandle => new Ort.OrtValueHandle(Value.DangerousGetHandle());

    internal SafeHandle Value { get; }
}
