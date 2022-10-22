namespace DKZKV.Kafka.Abstractions.Producer;

/// <summary>
/// Abstract class for custom producers middleware 
/// </summary>
public abstract class KafkaProducerMiddleware
{
    private KafkaProducerMiddleware _next;

    internal KafkaProducerMiddleware AddNext(KafkaProducerMiddleware middleware)
    {
        _next = middleware;
        return this;
    }

    /// <summary>
    /// Middleware logic
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="token"></param>
    /// <typeparam name="TMessage"></typeparam>
    /// <returns></returns>
    public abstract Task Handle<TMessage>(TMessage[] messages, CancellationToken token) where TMessage : IKafkaMessage;

    /// <summary>
    /// Next chain of middleware
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="token"></param>
    /// <typeparam name="TMessage"></typeparam>
    protected async Task Next<TMessage>(TMessage[] messages, CancellationToken token) where TMessage : IKafkaMessage
    {
        await _next.Handle(messages, token);
    }
}