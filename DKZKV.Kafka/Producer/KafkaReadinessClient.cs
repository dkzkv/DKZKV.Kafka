using Confluent.Kafka;
using DKZKV.Kafka.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DKZKV.Kafka.Producer;

internal class KafkaReadinessClient 
{
    private readonly IAdminClient _adminClient;
    public KafkaReadinessClient(IOptions<KafkaConnectionSettings> options,
        ILogger<KafkaReadinessClient> logger)
    {
        var settings = options.Value;
        var conf = new AdminClientConfig()
        {
            BootstrapServers = settings.Servers,
            SecurityProtocol = settings.SecurityProtocol,
            SaslMechanism = settings.SaslMechanism,
            SaslUsername = settings.SaslUsername,
            SaslPassword = settings.SaslPassword
        };
        _adminClient = new AdminClientBuilder(conf)
            .SetErrorHandler((_, error) => logger.LogError("Kafka admin error: {Error}",error.Reason))
            .Build();
    }


    public TopicMetadataResponse GetTopicMetadata(string topicName)
    {
        var topic = _adminClient.GetMetadata(topicName, TimeSpan.FromSeconds(10));
        if (topic.Topics.First().Error.IsError)
            return new TopicMetadataResponse(false);
        
        return new TopicMetadataResponse(true, topic.Topics.First().Partitions.Count);
    }
}