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

`OrtValue<T>` derives from `OrtValue` and uses the same native handle owner.
Construction snapshots the tensor shape once; typed data access and binding
do not allocate additional wrappers or copy the tensor data.

```csharp
using OnnxRuntimeSharp;

using var environment = new OrtEnv();
using var session = new OrtSession(environment, File.ReadAllBytes("mnist-8.onnx"));
using var input = new OrtValue<float>(new float[28 * 28], [1, 1, 28, 28]);
using var output = new OrtValue<float>(new float[10], [1, 10]);

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

## Bindings

The bindings use ONNX Runtime C API 28 and negotiate older API versions when
needed. One API table is used, with functions unavailable in the loaded runtime
left as null pointers. Calling an unavailable function fails hard and may
terminate the process; callers must only use APIs supported by their runtime.
Other versioned API tables and newer options or tensor types may have additional
runtime requirements.

## Execution providers

`Ort.GetAvailableExecutionProviders()` reports providers enabled in the native
build. `ExecutionProviders` checks runtime availability by running a small model
and validating its outputs, with CPU fallback disabled for accelerators (API 16+).

```csharp
var environment = OrtEnv.Instance();
var available = ExecutionProviders.FindAvailablePrioritizedExecutionProviders();
// Default order: TensorRT, CUDA, DirectML, OpenVINO, CPU, None.

using var options = new OrtSessionOptions();
foreach (var provider in available)
{
    provider.Append(environment, options);
}
using var session = new OrtSession(environment, model, options);
```

Pass a candidate list to change priority; use `ProbeExecutionProviders` for
per-provider diagnostics. Overloads without an environment use `OrtEnv.Instance()`;
do not dispose this shared instance. A successful probe does not guarantee support
for your model, and native crashes are not contained.
