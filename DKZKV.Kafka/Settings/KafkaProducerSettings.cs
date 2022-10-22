using DKZKV.Kafka.Abstractions;

namespace DKZKV.Kafka.Settings;

internal class KafkaProducerSettings<T> where T : IKafkaMessage
{
    public KafkaProducerSettings(string topic)
    {
        TopicName = topic;
        IsPartitioning = false;
        GetKey = null;
    }

    public KafkaProducerSettings(string topic, Func<T, object> getKey)
    {
        TopicName = topic;
        IsPartitioning = true;
        GetKey = getKey;
    }

    public string TopicName { get; }

    public bool IsPartitioning { get; }

#nullable enable
    public Func<T, object>? GetKey { get; }
#nullable disable
}