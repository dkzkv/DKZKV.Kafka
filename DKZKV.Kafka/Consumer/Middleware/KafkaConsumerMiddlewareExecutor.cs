using Confluent.Kafka;
using DKZKV.Kafka.Abstractions;
using DKZKV.Kafka.Abstractions.Consumer;
using Microsoft.Extensions.DependencyInjection;

namespace DKZKV.Kafka.Consumer.Middleware;

internal class KafkaConsumerMiddlewareExecutor 
{
    private readonly IServiceProvider _provider;

    public KafkaConsumerMiddlewareExecutor(IServiceProvider provider)
    {
        _provider = provider;
    }

    public  async Task Execute<TMessage>(IConsumeContext<TMessage> message, IConsumer<byte[], byte[]> consumer, CancellationToken token)
        where TMessage : class, IKafkaMessage
    {
        var commitConsumerMiddleware = new CommitConsumerMiddleware(consumer);
        var middlewares = new List<KafkaConsumerMiddleware> { commitConsumerMiddleware };
        middlewares.AddRange(_provider.GetServices<KafkaConsumerMiddleware>().Reverse());
        
        var chain = middlewares.Aggregate((next, pipeline) => pipeline.AddNext(next));
        await chain.Handle(message, token);
    }
}

