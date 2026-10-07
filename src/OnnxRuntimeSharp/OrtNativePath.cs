using System;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

unsafe ref struct OrtNativePath
{
    byte* _utf8;

    // The caller must keep the UTF-16 buffer alive for this scope.
    public OrtNativePath(string path, char* utf16Path)
    {
        _utf8 = null;
        if (OperatingSystem.IsWindows())
        {
            Pointer = (ushort*)utf16Path;
        }
        else
        {
            _utf8 = Utf8StringMarshaller.ConvertToUnmanaged(path);
            Pointer = (ushort*)_utf8;
        }
    }

    public ushort* Pointer { get; private set; }

    public void Dispose()
    {
        if (_utf8 is not null)
        {
            Utf8StringMarshaller.Free(_utf8);
            _utf8 = null;
        }
        Pointer = null;
    }
}
