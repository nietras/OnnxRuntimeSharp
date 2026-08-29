using System;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtRunOptionsTest
{
    [TestMethod]
    public void RunPropertiesRoundTrip()
    {
        using var options = new OrtRunOptions
        {
            LogSeverityLevel = Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR,
            LogVerbosityLevel = 1,
            Tag = "vision-request",
        };

        Assert.AreEqual(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR, options.LogSeverityLevel);
        Assert.AreEqual(1, options.LogVerbosityLevel);
        Assert.AreEqual("vision-request", options.Tag);
    }

    [TestMethod]
    public void TerminationCanBeRequestedAndReset()
    {
        using var options = new OrtRunOptions();

        options.RequestTermination();
        options.ResetTermination();
    }

    [TestMethod]
    public void ConfigEntriesCanBeAdded()
    {
        using var options = new OrtRunOptions();

        options.AddConfigEntry("disable_synchronize_execution_providers", "0");
    }

    [TestMethod]
    public void DisposedOptionsRejectOperations()
    {
        var options = new OrtRunOptions();
        options.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => options.RequestTermination());
    }

    [TestMethod]
    public void InvalidPropertiesAndConfigAreRejected()
    {
        using var options = new OrtRunOptions();

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => options.LogVerbosityLevel = -1);
        Assert.ThrowsExactly<ArgumentNullException>(() => options.Tag = null!);
        Assert.ThrowsExactly<ArgumentException>(() => options.AddConfigEntry("", "value"));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.AddConfigEntry("key", null!));
    }
}
