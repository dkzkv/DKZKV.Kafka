using DKZKV.Kafka.Abstractions.Producer;
using Microsoft.Extensions.DependencyInjection;

namespace DKZKV.Kafka.Producer.Middleware;

internal class KafkaProducerMiddlewareExecutor : KafkaProducerMiddleware
{
    private readonly IServiceProvider _provider;

    public KafkaProducerMiddlewareExecutor(IServiceProvider provider)
    {
        _provider = provider;
    }

    public override async Task Handle<TMessage>(TMessage[] messages, CancellationToken token)
    {
        var chain = _provider.GetServices<KafkaProducerMiddleware>()
            .Where(x => x is not KafkaProducerMiddlewareExecutor)
            .Reverse()
            .Aggregate((next, pipeline) => pipeline.AddNext(next));

        await chain.Handle(messages, token);
    }
}