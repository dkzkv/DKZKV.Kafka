namespace DKZKV.Kafka.Abstractions.Serialization;

/// <summary>
/// Consumer message deserializer
/// </summary>
public interface IKafkaDeserializer
{
    /// <summary>
    /// Deserialize
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="TMessage"></typeparam>
    /// <returns></returns>
    TMessage Deserialize<TMessage>(byte[] data) where TMessage : class, IKafkaMessage;
}