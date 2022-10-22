using DKZKV.Kafka.Abstractions;
using DKZKV.Kafka.Abstractions.Consumer;

namespace DKZKV.Kafka.Consumer;

internal class ConsumeContext<TMessage> : IConsumeContext<TMessage> where TMessage : class, IKafkaMessage
{
    public ConsumeContext(string topicName, int partition, long offset, IEnumerable<TMessage> messages)
    {
        TopicName = topicName;
        Partition = partition;
        Offset = offset;
        Messages = messages;
    }

    public string TopicName { get; }
    public int Partition { get; }
    public long Offset { get; }
    public IEnumerable<TMessage> Messages { get; }
}