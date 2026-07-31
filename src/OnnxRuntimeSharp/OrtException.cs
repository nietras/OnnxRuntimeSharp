using System;

namespace OnnxRuntimeSharp;

public sealed class OrtException(Ort.OrtErrorCode errorCode, string message) : Exception(message)
{
    public Ort.OrtErrorCode ErrorCode { get; } = errorCode;
}
