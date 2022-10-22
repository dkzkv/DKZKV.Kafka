using DKZKV.Kafka.Abstractions.Serialization;

namespace DKZKV.Kafka.Serialization;

/// <inheritdoc />
public class DefaultJsonSerializer : IKafkaSerializer 
{
    /// <inheritdoc />
    public byte[] Serialize<TMessage>(TMessage data)
    {
        return Utf8Json.JsonSerializer.Serialize(data);
    }
}