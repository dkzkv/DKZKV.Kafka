using Confluent.Kafka;
using DKZKV.Kafka.Abstractions;
using DKZKV.Kafka.Consumer.Middleware;
using DKZKV.Kafka.Exceptions;
using DKZKV.Kafka.Producer;
using DKZKV.Kafka.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DKZKV.Kafka.Consumer;

internal class KafkaConsumerService<TMessage> : BackgroundService where TMessage : class, IKafkaMessage
{
    private readonly ConsumeContextFactory<TMessage> _contextFactory;
    private readonly KafkaConsumerSettings<TMessage> _consumerSettings;
    private readonly ILogger<KafkaConsumerService<TMessage>> _logger;
    private readonly KafkaConsumerMiddlewareExecutor _middlewareExecutor;
    private readonly ConsumerFactory<TMessage> _consumerFactory;
    private readonly KafkaReadinessClient _readinessClient;

    public KafkaConsumerService(ILogger<KafkaConsumerService<TMessage>> logger,
        ConsumeContextFactory<TMessage> contextFactory,
        KafkaConsumerSettings<TMessage> consumerSettings,
        KafkaConsumerMiddlewareExecutor middlewareExecutor,
        ConsumerFactory<TMessage> consumerFactory,
        KafkaReadinessClient readinessClient)
    {
        _logger = logger;
        _consumerSettings = consumerSettings;
        _middlewareExecutor = middlewareExecutor;
        _consumerFactory = consumerFactory;
        _readinessClient = readinessClient;
        _contextFactory = contextFactory;
    }

    private const int ConsumeTimeout = 100;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var topicMetadata = _readinessClient.GetTopicMetadata(_consumerSettings.TopicName);
        if (!topicMetadata.IsTopicCreated)
            _logger.LogWarning("Topic: {Name} does not exist", _consumerSettings.TopicName);
        else if (topicMetadata.PartitionsCount != _consumerSettings.ConsumersCount)
            _logger.LogWarning("Partition count: {Partitions} does not equal to consumers count: {Consumers}", topicMetadata.PartitionsCount,
                _consumerSettings.ConsumersCount);

        var consumerTasks = new Task[_consumerSettings.ConsumersCount];
        for (int i = 0; i < _consumerSettings.ConsumersCount; i++)
        {
            consumerTasks[i] = RunConsumption(stoppingToken);
        }

        return Task.WhenAll(consumerTasks);
    }

    private Task RunConsumption(CancellationToken token)
    {
        return Task.Run(async () =>
        {
            using var consumer = _consumerFactory.Create();
            consumer.Subscribe(_consumerSettings.TopicName);
            var rawMessages = new List<ConsumeResult<byte[], byte[]>>(_consumerSettings.BatchSize);
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(ConsumeTimeout);
                    if (consumeResult?.IsPartitionEOF ?? true)
                    {
                        if (rawMessages.Any())
                            await ExecuteAndCommit(rawMessages, consumer, token);

                        await Task.Delay(_consumerSettings.EofConsumeDelayInMls, token);
                    }
                    else
                    {
                        rawMessages.Add(consumeResult);
                        if (rawMessages.Count >= _consumerSettings.BatchSize)
                            await ExecuteAndCommit(rawMessages, consumer, token);
                    }
                }
                catch (KafkaDeserializationExceptions e)
                {
                    _logger.LogError(e, "Batch consuming deserialization error");
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "An error occured while processing '{Topic}' consumption", _consumerSettings.TopicName);
                    if (rawMessages.Any())
                    {
                        ResetOffset(rawMessages, consumer);
                        rawMessages.Clear();
                    }
                }
            }
        }, token);
    }

    private async Task ExecuteAndCommit(IList<ConsumeResult<byte[], byte[]>> rawMessages, IConsumer<byte[], byte[]> consumer, CancellationToken token)
    {
        await _middlewareExecutor.Execute(_contextFactory.Create(rawMessages.ToArray()), consumer, token);
        rawMessages.Clear();
    }

    private void ResetOffset(IList<ConsumeResult<byte[], byte[]>> rawMessages, IConsumer<byte[], byte[]> consumer)
    {
        var earliestOffset = rawMessages.Min(o => o.Offset.Value);
        var topic = rawMessages.First().Topic;
        var partition = rawMessages.First().Partition;
        consumer.Assign(new TopicPartitionOffset(topic, partition, earliestOffset));
    }
}