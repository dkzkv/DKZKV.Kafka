namespace DKZKV.Kafka.Abstractions.Serialization;

/// <summary>
/// Producer message serializer
/// </summary>
public interface IKafkaSerializer
{

    /// <summary>
    /// Serialize
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="TMessage"></typeparam>
    /// <returns></returns>
    byte[] Serialize<TMessage>(TMessage data);
}