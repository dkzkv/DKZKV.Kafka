using DKZKV.Kafka.Abstractions;
using DKZKV.Kafka.Abstractions.Producer;
using DKZKV.Kafka.Producer.Middleware;

namespace DKZKV.Kafka.Producer;

internal class KafkaProducer<TMessage> : IKafkaProducer<TMessage> where TMessage : IKafkaMessage
{
    private readonly KafkaProducerMiddlewareExecutor _executor;

    public KafkaProducer(KafkaProducerMiddlewareExecutor executor)
    {
        _executor = executor;
    }

    public async Task Produce(TMessage messages, CancellationToken token)
    {
        await _executor.Handle(new[] { messages }, token);
    }

    public async Task Produce(TMessage[] messages, CancellationToken token)
    {
        await _executor.Handle(messages, token);
    }
}