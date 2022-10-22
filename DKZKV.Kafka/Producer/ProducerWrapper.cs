using Confluent.Kafka;
using DKZKV.Kafka.Abstractions;
using DKZKV.Kafka.Abstractions.Serialization;
using DKZKV.Kafka.Exceptions;
using DKZKV.Kafka.Settings;
using Microsoft.Extensions.Logging;

namespace DKZKV.Kafka.Producer;

internal class ProducerWrapper<TMessage> where TMessage : IKafkaMessage
{
    private readonly IKafkaSerializer _serializer;
    private readonly IProducer<byte[], byte[]> _producer;
    private readonly KafkaProducerSettings<TMessage> _producerSettings;

    public ProducerWrapper(KafkaProducerSettings<TMessage> producerSettings,
        ILogger<ProducerWrapper<TMessage>> logger,
        ProducerInstanceProvider instanceProvider,
        KafkaReadinessClient readinessClient,
        IKafkaSerializer serializer)
    {
        var topicMetadata = readinessClient.GetTopicMetadata(producerSettings.TopicName);
        if(!topicMetadata.IsTopicCreated)
            logger.LogWarning("Topic: {Name} does not exist", producerSettings.TopicName);
        
        _producer = instanceProvider.GetInstance();
        _producerSettings = producerSettings;
        _serializer = serializer;
    }

    private static string ConsumerHeader => "kafka-handler-id";

    public Task ProduceAsync(TMessage[] messages, CancellationToken token)
    {
        if (!messages.Any())
            throw new InvalidOperationException("Sequence contains no elements");

        if (messages.Any(o=>o is null))
            throw new ArgumentNullException($"Kafka message cannot be null");
        
        if (messages.Count() == 1)
            return SingleProduce(messages.First(), token);

        return BatchProduce(messages, token);
    }

    private async Task SingleProduce(TMessage message, CancellationToken token)
    {
        var result = await _producer.ProduceAsync(_producerSettings.TopicName, PrepareMessage(Guid.NewGuid(), message), token);
        if (result.Status != PersistenceStatus.Persisted)
            throw new KafkaProducerExceptions("Message possibly has not been sent");
    }

    private async Task BatchProduce(TMessage[] messages, CancellationToken token)
    {
        var messageHandlers = GenerateMessageHandlers(messages);
        void DeliveryHandler(DeliveryReport<byte[], byte[]> report)
        {
            if (_producerSettings.TopicName == report.Topic)
            {
                var handlerId = GetHandlerId(report);
                if (messageHandlers.ContainsKey(handlerId))
                    messageHandlers[handlerId].TaskCompletionSource.SetResult(report.Status == PersistenceStatus.Persisted);
            }
        }
        
        foreach (var messageHandler in messageHandlers)
        {
            _producer.Produce(_producerSettings.TopicName, PrepareMessage(messageHandler.Key, messageHandler.Value.Message), DeliveryHandler);
        }

        var deliveryTasks = messageHandlers.Select(o => o.Value.TaskCompletionSource.Task).ToArray();
        await Task.WhenAny(Task.Delay(-1, token),
            Task.WhenAll(deliveryTasks));
        
        token.ThrowIfCancellationRequested();
        var deliveryResults = await Task.WhenAll(deliveryTasks);
        if(deliveryResults.Any(o => o == false))
            throw new KafkaProducerExceptions("Messages possibly has not been sent");
    }
    
    private Dictionary<Guid, (TaskCompletionSource<bool> TaskCompletionSource, TMessage Message)> GenerateMessageHandlers(TMessage[] messages)
    {
        var resetEvents = new Dictionary<Guid, (TaskCompletionSource<bool> TaskCompletionSource, TMessage Message)>(messages.Count());
        for (int i = 0; i < messages.Count(); i++)
        {
            resetEvents.Add(Guid.NewGuid(), (new TaskCompletionSource<bool>(), messages[i]));
        }

        return resetEvents;
    }

    private Guid GetHandlerId(DeliveryReport<byte[], byte[]> report) => new(report.Headers.First(o => o.Key == ConsumerHeader).GetValueBytes());
    
    private Message<byte[], byte[]> PrepareMessage(Guid handlerId, TMessage message)
    {
        return new Message<byte[], byte[]>
        {
            Headers = new Headers()
            {
                new Header(ConsumerHeader, handlerId.ToByteArray())
            },
            Key = _producerSettings.IsPartitioning
                ? _serializer.Serialize(_producerSettings.GetKey!.Invoke(message))
                : null,
            Value = _serializer.Serialize(message)
        };
    }
}