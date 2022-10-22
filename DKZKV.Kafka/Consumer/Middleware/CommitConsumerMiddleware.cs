using Confluent.Kafka;
using DKZKV.Kafka.Abstractions.Consumer;

namespace DKZKV.Kafka.Consumer.Middleware;

internal class CommitConsumerMiddleware : KafkaConsumerMiddleware
{
    private readonly IConsumer<byte[], byte[]> _consumer;
    public CommitConsumerMiddleware(IConsumer<byte[], byte[]> consumer)
    {
        _consumer = consumer;
    }

    public override Task Handle<TMessage>(IConsumeContext<TMessage> context, CancellationToken token)
    {
        _consumer.Commit();
        return Task.CompletedTask;
    }
}