using System;
using System.Linq;
using System.Threading;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class InferenceSafetyTest
{
    [TestMethod]
    public void CachedBindingsRunWithoutManagedAllocations()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();
        var inputs = new[] { session.CreateInputBinding(0, input) };
        var outputs = new[] { session.CreateOutputBinding(0, output) };

        for (var warmup = 0; warmup < 3; ++warmup)
        {
            session.Run(inputs, outputs);
        }

        var allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();
        for (var iteration = 0; iteration < 1_000; ++iteration)
        {
            session.Run(inputs, outputs);
        }

        Assert.AreEqual(0, GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore);
    }

    [TestMethod]
    public void BindingCreationDoesNotAllocate()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();
        _ = session.CreateInputBinding(0, input);
        _ = session.CreateOutputBinding(0, output);

        var allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();
        for (var iteration = 0; iteration < 1_000; ++iteration)
        {
            var inputBinding = session.CreateInputBinding(0, input);
            var outputBinding = session.CreateOutputBinding(0, output);
            _ = inputBinding.Info;
            _ = outputBinding.Info;
        }

        Assert.AreEqual(0, GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore);
    }

    [TestMethod]
    public void ConcurrentRunsOnSharedSessionComplete()
    {
        using var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        Exception? failure = null;
        var threads = Enumerable.Range(0, 4).Select(_ => new Thread(() =>
        {
            try
            {
                using var input = TestData.CreateMnistInput();
                using var output = TestData.CreateMnistOutput();
                for (var iteration = 0; iteration < 20; ++iteration)
                {
                    session.Run(input, output);
                }
            }
            catch (Exception exception)
            {
                Interlocked.CompareExchange(ref failure, exception, null);
            }
        })).ToArray();

        foreach (var thread in threads)
        {
            thread.Start();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.IsNull(failure);
    }

    [TestMethod]
    public void DisposedSessionRejectsInference()
    {
        using var environment = new OrtEnvironment();
        var session = TestData.CreateMnistSession(environment);
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();
        session.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => session.Run(input, output));
    }

    [TestMethod]
    public void SessionRetainsEnvironmentUntilSessionDisposal()
    {
        var environment = new OrtEnvironment();
        using var session = TestData.CreateMnistSession(environment);
        environment.Dispose();
        using var input = TestData.CreateMnistInput();
        using var output = TestData.CreateMnistOutput();

        session.Run(input, output);
    }

    [TestMethod]
    public void IoBindingCanBeDisposedAfterSession()
    {
        using var environment = new OrtEnvironment();
        var session = TestData.CreateMnistSession(environment);
        var binding = session.CreateIoBinding();

        session.Dispose();
        binding.Dispose();
    }
}
