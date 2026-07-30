using System;

namespace OnnxRuntimeSharp;

public sealed class OrtException(string message) : Exception(message);
