using DKZKV.Kafka;
using DKZKV.Kafka.Abstractions;
using KafkaSampleService.ConsumerMiddlewares;
using KafkaSampleService.Consumers;
using KafkaSampleService.Models;
using KafkaSampleService.ProducerMiddlewares;
using KafkaSampleService.Producers;


namespace KafkaSampleService;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private const string TopicForTestMessages = "test-topic";
    private const string TopicForTestMessagesWithKey = "test-topic-with-key";

    private const int BatchSize = 10;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKafka(conf =>
        {
            // Override producer serializer
            //conf.SetProducerSerializer(new DefaultJsonSerializer());

            // Extended producer settings
            conf.SetGlobalProducersSettings(set =>
            {
                set.Compression = MessageCompression.Snappy;
                set.ProduceTimeout = TimeSpan.FromSeconds(10);
                set.QueueBufferingMaxMessages = 1_000_000;
            });

            conf.AddProducer<TestMessage>(TopicForTestMessages);
            conf.AddProducer<TestMessageWithKey>(TopicForTestMessagesWithKey, message => message.KeyPartOne + message.KeyPartTwo);

            conf.AddProducerMiddleware<ErrorHandlerMiddleware>();
            conf.AddProducerMiddleware<LoggingMiddleware>();

            conf.SetConsumersGroupId("foo");
            conf.AddConsumer<TestMessageConsumer, TestMessage>(TopicForTestMessages, 100);
            conf.AddConsumer<TestMessageWithKeyConsumer, TestMessageWithKey>(set =>
            {
                set.TopicName = TopicForTestMessagesWithKey;
                //set.GroupId = "foo1"; // override consumer group name
                set.BatchSize = BatchSize;
                set.ConsumersCount = 1; // Set consumers count for parallel partition reading
                //set.EofConsumeDelayInMls = 1000;
            });

            conf.AddConsumerMiddleware<ConsumerErrorHandlerMiddleware>();
        }, _configuration);

        // Background service witch sends messages with intervals
        services.AddHostedService<KafkaProducerTestMessagesWorker>(); //TestMessage
        services.AddHostedService<KafkaProducerTestMessagesWithKeyWorker>(); //TestMessageWithKey
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider sp)
    {
    }
}