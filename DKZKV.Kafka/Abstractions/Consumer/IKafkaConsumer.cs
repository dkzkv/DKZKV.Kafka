namespace DKZKV.Kafka.Abstractions.Consumer;

/// <summary>
/// Consumer for bach message reading
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface IKafkaConsumer<TMessage> where TMessage : class, IKafkaMessage
{
    /// <summary>
    /// Invoke when new messages received. Batch can be less than batch size
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task Consume(IConsumeContext<TMessage> context, CancellationToken token);
}



