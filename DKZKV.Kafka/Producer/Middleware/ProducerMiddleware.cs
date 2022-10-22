using DKZKV.Kafka.Abstractions.Producer;
using Microsoft.Extensions.DependencyInjection;

namespace DKZKV.Kafka.Producer.Middleware;

internal class ProducerMiddleware : KafkaProducerMiddleware
{
    private readonly IServiceProvider _serviceProvider;

    public ProducerMiddleware(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task Handle<TMessage>(TMessage[] messages, CancellationToken token) 
    {
        var producer = _serviceProvider.GetRequiredService<ProducerWrapper<TMessage>>();
        await producer.ProduceAsync(messages, token);
    }
}