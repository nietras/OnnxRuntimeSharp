using System;

namespace OnnxRuntimeSharp.Test;

[TestClass]
public class OrtRunOptionsTest
{
    [TestMethod]
    public void RunOptionsRoundTrip()
    {
        using var options = new OrtRunOptions();
        options.SetLogSeverityLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR);
        options.SetLogVerbosityLevel(1);
        options.SetTag("vision-request");

        Assert.AreEqual(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR, options.GetLogSeverityLevel());
        Assert.AreEqual(1, options.GetLogVerbosityLevel());
        Assert.AreEqual("vision-request", options.GetTag());
    }

    [TestMethod]
    [DataRow("request-42")]
    [DataRow("request-\u00e6-\u03bb-\u65e5\u672c")]
    [DataRow("")]
    public void TagCanBeChanged(string tag)
    {
        using var options = new OrtRunOptions();
        options.SetTag("previous");
        options.SetTag(tag);

        Assert.AreEqual(tag, options.GetTag());
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
        Assert.ThrowsExactly<ObjectDisposedException>(() => options.GetLogVerbosityLevel());
        Assert.ThrowsExactly<ObjectDisposedException>(() => options.SetLogVerbosityLevel(0));
        Assert.ThrowsExactly<ObjectDisposedException>(() => options.GetLogSeverityLevel());
        Assert.ThrowsExactly<ObjectDisposedException>(() => options.SetLogSeverityLevel(Ort.OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR));
        Assert.ThrowsExactly<ObjectDisposedException>(() => options.GetTag());
        Assert.ThrowsExactly<ObjectDisposedException>(() => options.SetTag("request"));
    }

    [TestMethod]
    public void InvalidOptionsAndConfigAreRejected()
    {
        using var options = new OrtRunOptions();

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => options.SetLogVerbosityLevel(-1));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.SetTag(null!));
        Assert.ThrowsExactly<ArgumentException>(() => options.AddConfigEntry("", "value"));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.AddConfigEntry("key", null!));
    }
}
