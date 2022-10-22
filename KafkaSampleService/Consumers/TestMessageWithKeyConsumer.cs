using DKZKV.Kafka.Abstractions.Consumer;
using KafkaSampleService.Models;

namespace KafkaSampleService.Consumers;

public class TestMessageWithKeyConsumer : IKafkaConsumer<TestMessageWithKey>
{
    private readonly ILogger<TestMessageWithKeyConsumer> _logger;

    public TestMessageWithKeyConsumer(ILogger<TestMessageWithKeyConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(IConsumeContext<TestMessageWithKey> context, CancellationToken token)
    {
        foreach (var message in context.Messages)
        {
            _logger.LogInformation("Message with key received: {Message}",message.TestValue);
        }
        return Task.CompletedTask;
    }
}