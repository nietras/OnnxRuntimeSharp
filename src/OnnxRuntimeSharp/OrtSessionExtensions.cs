using System;
using System.Diagnostics;
using static OnnxRuntimeSharp.Ort;

namespace OnnxRuntimeSharp;

public static class OrtSessionExtensions
{
    /// <summary>
    /// Warmup <see cref="OrtSession"/> with <paramref name="warmupRuns"/> runs
    /// to ensure it is fully ready for inference. Creating the <see
    /// cref="OrtSession"/> will not always load all native dependencies so
    /// warmup is needed to ensure that. Additionally, first run may be very
    /// slow so warmup ensures that ready for the first run during production
    /// for example.
    /// </summary>
    /// <remarks>
    /// Note this will allocate dummy input for warmup on the managed heap
    /// depending on inputs defined in the model.
    /// </remarks>
    /// <param name="session">The inference session to warmup.</param>
    /// <param name="warmupRuns">Number of times to execute inference as warmup.
    /// If less than 1 nothing is done.</param>
    /// <param name="warmupBatchSize">Positive batch size used for a dynamic
    /// leading input dimension. Fixed dimensions and scalar inputs are preserved.</param>
    public unsafe static void Warmup(this OrtSession session,
        int warmupRuns = 1, int warmupBatchSize = 1)
    {
        if (warmupRuns < 1) { return; }
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(warmupBatchSize);

        OrtAllocator* allocator = null;
        GetAllocatorWithDefaultOptions(&allocator).Ok();

        var inputTensorInfos = session.Inputs.Span;
        var outputTensorInfos = session.Outputs.Span;

        var inputCount = inputTensorInfos.Length;
        var inputNamePtrs = stackalloc sbyte*[inputCount];
        OrtValueHandle* inputOrtValues = stackalloc OrtValueHandle[inputCount];
        try
        {
            for (var i = 0; i < inputCount; i++)
            {
                var tensorInfo = inputTensorInfos[i];
                inputNamePtrs[i] = tensorInfo.NamePointer;
                inputOrtValues[i] = CreateOrtValueTensor(allocator, tensorInfo, warmupBatchSize);
            }

            var outputCount = outputTensorInfos.Length;
            var outputNamePtrs = stackalloc sbyte*[outputCount];
            for (var i = 0; i < outputCount; i++)
            {
                var tensorInfo = outputTensorInfos[i];
                outputNamePtrs[i] = tensorInfo.NamePointer;
            }

            Trace.WriteLine($"Performing {warmupRuns} warmup run(s) on {nameof(OrtSession)}");
            for (var i = 0; i < warmupRuns; i++)
            {
                TestRun(session, runOptions: null, inputNamePtrs, inputOrtValues, inputCount,
                        outputNamePtrs, outputCount);
            }
        }
        finally
        {
            for (var i = 0; i < inputCount; i++)
            {
                var ortValueHandle = inputOrtValues[i];
                if (!ortValueHandle.IsNull) { ReleaseValue(ortValueHandle); }
            }
        }

        static OrtValueHandle CreateOrtValueTensor(OrtAllocator* allocator, OrtTensorInfo tensorInfo, int batchSize)
        {
            var dimensionsLength = tensorInfo.Dimensions.Length;
            var dimensionsPtr = stackalloc long[dimensionsLength];
            tensorInfo.Dimensions.Span.CopyTo(new(dimensionsPtr, dimensionsLength));
            if (dimensionsLength > 0 && dimensionsPtr[0] < 0)
            {
                dimensionsPtr[0] = batchSize;
            }
            OrtValueHandle valueHandle;
            CreateTensorAsOrtValue(allocator, dimensionsPtr, (nuint)dimensionsLength,
                                   tensorInfo.ElementType, &valueHandle).Ok();
            return valueHandle;
        }
    }

    // Optimized Run path without heap alloc return values, instead only looks at pointer
    internal static unsafe void TestRun(this OrtSession session, OrtRunOptions? runOptions,
        sbyte** inputNames, OrtValueHandle* inputs, int inputCount,
        sbyte** outputNames, int outputCount)
    {
        // Pointer to array of OnnxValue IntPtrs/handles
        var outputValuesPtr = stackalloc OrtValueHandle[outputCount];

        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();

        Run(session.Handle, runOptions?.Handle ?? default, inputNames,
            inputs, (nuint)inputCount, outputNames, (nuint)outputCount, outputValuesPtr).Ok();

        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        try
        {
            if (allocatedBytes > 0)
            {
                Trace.WriteLine($"OrtSession.Run allocated {allocatedBytes} managed bytes on the current thread.");
            }
            for (var i = 0; i < outputCount; i++)
            {
                GetTensorMutableData(outputValuesPtr[i], out var outputValueIntPtr).Ok();
                var outputValuePtr = (float*)outputValueIntPtr;
                // Output not used for anything just touched to be sure it's not optimized away
            }
        }
        finally
        {
            for (var j = 0; j < outputCount; j++)
            {
                ReleaseValue(outputValuesPtr[j]);
            }
        }
    }
}
