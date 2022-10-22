using DKZKV.Kafka.Abstractions.Consumer;
using Microsoft.Extensions.DependencyInjection;

namespace DKZKV.Kafka.Consumer.Middleware;

internal class ConsumerMiddleware : KafkaConsumerMiddleware
{
    private readonly IServiceProvider _serviceProvider;

    public ConsumerMiddleware(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public override async Task Handle<TMessage>(IConsumeContext<TMessage> context, CancellationToken token) 
    {
        var consumer = _serviceProvider.GetRequiredService<IKafkaConsumer<TMessage>>();
        await consumer.Consume(context, token);
        await Next(context, token);
    }
}