using System.Collections.Generic;

namespace OnnxRuntimeSharp;

public sealed class OrtModelMetadata
{
    internal OrtModelMetadata(
        string producerName,
        string graphName,
        string graphDescription,
        string domain,
        string description,
        long version,
        IReadOnlyDictionary<string, string> customMetadata)
    {
        ProducerName = producerName;
        GraphName = graphName;
        GraphDescription = graphDescription;
        Domain = domain;
        Description = description;
        Version = version;
        CustomMetadata = customMetadata;
    }

    public string ProducerName { get; }
    public string GraphName { get; }
    public string GraphDescription { get; }
    public string Domain { get; }
    public string Description { get; }
    public long Version { get; }
    public IReadOnlyDictionary<string, string> CustomMetadata { get; }
}
