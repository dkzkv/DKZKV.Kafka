using DKZKV.Kafka.Abstractions;
using DKZKV.Kafka.Abstractions.Serialization;

namespace DKZKV.Kafka.Serialization;

/// <inheritdoc />
public class DefaultJsonDeserializer : IKafkaDeserializer
{
    /// <inheritdoc />
    public TMessage Deserialize<TMessage>(byte[] data) where TMessage : class, IKafkaMessage
    {
        return Utf8Json.JsonSerializer.Deserialize<TMessage>(data);
    }
}