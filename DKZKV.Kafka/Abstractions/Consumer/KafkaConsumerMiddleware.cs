namespace DKZKV.Kafka.Abstractions.Consumer;

/// <summary>
/// Abstract class for custom producers middleware 
/// </summary>
public abstract class KafkaConsumerMiddleware
{
    private KafkaConsumerMiddleware _next;
    
    internal KafkaConsumerMiddleware AddNext(KafkaConsumerMiddleware middleware)
    {
        _next = middleware;
        return this;
    }

    /// <summary>
    /// Middleware logic
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <typeparam name="TMessage"></typeparam>
    /// <returns></returns>
    public abstract Task Handle<TMessage>(IConsumeContext<TMessage> context, CancellationToken token) where TMessage : class, IKafkaMessage;


    /// <summary>
    /// Next chain of middleware
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <typeparam name="TMessage"></typeparam>
    protected async Task Next<TMessage>(IConsumeContext<TMessage> context, CancellationToken token) where TMessage : class, IKafkaMessage
    {
        await _next.Handle(context, token);
    }
}