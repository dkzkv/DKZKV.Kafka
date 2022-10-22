using Confluent.Kafka;
using DKZKV.Kafka.Abstractions.Producer;
using DKZKV.Kafka.Settings;
using DKZKV.Kafka.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DKZKV.Kafka.Producer;

internal class ProducerInstanceProvider 
{
    private readonly IProducer<byte[], byte[]> _producer;
    
    public ProducerInstanceProvider(IOptions<KafkaConnectionSettings> options,
        IKafkaGlobalProducerSettings globalProducerSettings,
        ILogger<ProducerInstanceProvider> logger)
    {
        var settings = options.Value;
        var config = new ProducerConfig
        {
            MessageTimeoutMs = (int)globalProducerSettings.ProduceTimeout.TotalMilliseconds,
            QueueBufferingMaxMessages = globalProducerSettings.QueueBufferingMaxMessages,
            CompressionType = (CompressionType)(int)globalProducerSettings.Compression,

            BootstrapServers = settings.Servers,
            SecurityProtocol = settings.SecurityProtocol,
            SaslMechanism = settings.SaslMechanism,
            SaslUsername = settings.SaslUsername,
            SaslPassword = settings.SaslPassword
        };
        _producer = new ProducerBuilder<byte[], byte[]>(config)
            .SetErrorHandler((_, error) => logger.LogError("Kafka producer error: {Error}",error.Reason))
            .Build();
    }
    
    public IProducer<byte[], byte[]> GetInstance()
    {
        return _producer;
    }
}