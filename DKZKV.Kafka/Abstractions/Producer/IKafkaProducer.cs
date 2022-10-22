namespace DKZKV.Kafka.Abstractions.Producer;

/// <summary>
/// Kafka message producer 
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface IKafkaProducer<in TMessage> where TMessage : IKafkaMessage
{
    /// <summary>
    /// Send message
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Produce(TMessage messages, CancellationToken token = default);

    /// <summary>
    /// Send messages
    /// </summary>
    /// <param name="messages">сообщение</param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Produce(TMessage[] messages, CancellationToken token = default);
}