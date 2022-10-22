using Confluent.Kafka;
using DKZKV.Kafka.Abstractions;
using DKZKV.Kafka.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DKZKV.Kafka.Consumer;

internal class ConsumerFactory<TMessage> where TMessage : class, IKafkaMessage
{
    private readonly ILogger<ConsumerFactory<TMessage>> _logger;
    private readonly ConsumerConfig _config;
    
    public ConsumerFactory(IOptions<KafkaConnectionSettings> kafkaSettings,
        KafkaConsumerSettings<TMessage> consumerSettings,
        ILogger<ConsumerFactory<TMessage>> logger)
    {
        _logger = logger;
        var settings = kafkaSettings.Value;
        _config = new ConsumerConfig
        {
            GroupId = consumerSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnablePartitionEof = true,
            AllowAutoCreateTopics = false,
            MaxPollIntervalMs = 300_000, //24h max interval
            //SessionTimeoutMs = 45000,
            
            BootstrapServers = settings.Servers,
            SecurityProtocol = settings.SecurityProtocol,
            SaslMechanism = settings.SaslMechanism,
            SaslUsername = settings.SaslUsername,
            SaslPassword = settings.SaslPassword
        };
    }
    
    public IConsumer<byte[], byte[]> Create()
    {
        return new ConsumerBuilder<byte[], byte[]>(_config)
            .SetErrorHandler((_, error) => _logger.LogError("Kafka consumer error: {Error}",error.Reason))
             .Build();
    }
}