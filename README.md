# OnnxRuntimeSharp

![.NET](https://img.shields.io/badge/net10.0-5C2D91?logo=.NET&labelColor=gray)
![C#](https://img.shields.io/badge/C%23-14.0-239120?labelColor=gray)
[![Build Status](https://github.com/nietras/OnnxRuntimeSharp/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/nietras/OnnxRuntimeSharp/actions/workflows/dotnet.yml)
[![Super-Linter](https://github.com/nietras/OnnxRuntimeSharp/actions/workflows/super-linter.yml/badge.svg)](https://github.com/marketplace/actions/super-linter)
[![NuGet](https://img.shields.io/nuget/v/OnnxRuntimeSharp?color=purple)](https://www.nuget.org/packages/OnnxRuntimeSharp/)
[![Release](https://img.shields.io/github/v/release/nietras/OnnxRuntimeSharp)](https://github.com/nietras/OnnxRuntimeSharp/releases/)
[![License](https://img.shields.io/github/license/nietras/OnnxRuntimeSharp)](https://github.com/nietras/OnnxRuntimeSharp/blob/main/LICENSE)

Low-level ONNX Runtime C API interop in modern C#. Cross-platform, trimmable,
and AOT/NativeAOT compatible.

## Example

The application supplies the native ONNX Runtime runtime package and owns the
managed input and output buffers. Tensors pin those buffers once, so steady
state inference does not allocate managed memory.

```csharp
using OnnxRuntimeSharp;

using var environment = new OrtEnvironment();
using var session = new OrtSession(environment, File.ReadAllBytes("mnist-8.onnx"));
using var input = new OrtTensor<float>(new float[28 * 28], [1, 1, 28, 28]);
using var output = new OrtTensor<float>(new float[10], [1, 10]);

session.Run(input, output);
```

## Profiling

Enable profiling before creating the session, run inference, and finish the
profile to retrieve the trace file path.

```csharp
using var options = new OrtSessionOptions();
options.EnableProfiling("mnist-profile");
using var session = new OrtSession(environment, model, options);
// Run inference.
var profilePath = session.EndProfiling();
```

The `OnnxRuntimeSharp.Profiler` project runs the bundled `mnist-8.onnx` model
and emits an ONNX Runtime JSON trace.

## Building

```powershell
dotnet test
dotnet run --project src\OnnxRuntimeSharp.Profiler
```

## License

This project is licensed under the [MIT license](LICENSE).
