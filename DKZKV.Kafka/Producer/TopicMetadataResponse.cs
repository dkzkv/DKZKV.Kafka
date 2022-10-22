namespace DKZKV.Kafka.Producer;

internal struct TopicMetadataResponse
{
    public TopicMetadataResponse(bool isTopicCreated, int partitionsCount=0)
    {
        IsTopicCreated = isTopicCreated;
        PartitionsCount = partitionsCount;
    }

    public bool IsTopicCreated { get; }
    public int PartitionsCount { get; }
}