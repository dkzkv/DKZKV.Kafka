using Confluent.Kafka;
using DKZKV.Kafka.Abstractions;
using DKZKV.Kafka.Abstractions.Serialization;
using DKZKV.Kafka.Exceptions;

namespace DKZKV.Kafka.Consumer;

internal class ConsumeContextFactory<TMessage>  where TMessage : class, IKafkaMessage
{
    private readonly IKafkaDeserializer _deserializer;
    public ConsumeContextFactory(IKafkaDeserializer deserializer)
    {
        _deserializer = deserializer;
    }

    public ConsumeContext<TMessage> Create(ConsumeResult<byte[], byte[]>[] rawMessages)
    {
        var lastOffset = rawMessages.Max(o => o.Offset.Value);
        var topicName = rawMessages.First().Topic;
        var partition = rawMessages.First().Partition;
        return new ConsumeContext<TMessage>(topicName, partition, lastOffset, rawMessages.Select(Deserializers));
    }
    
    private TMessage Deserializers(ConsumeResult<byte[], byte[]> result)
    {
        try
        {
            return _deserializer.Deserialize<TMessage>(result.Message.Value);
        }
        catch (Exception e)
        {
            throw new KafkaDeserializationExceptions(result.Topic, result.Offset.Value, e);
        }
    }
}