using DKZKV.Kafka.Abstractions.Consumer;

namespace KafkaSampleService.ConsumerMiddlewares;

public class ConsumerErrorHandlerMiddleware : KafkaConsumerMiddleware
{
    private readonly ILogger<ConsumerErrorHandlerMiddleware> _logger;

    public ConsumerErrorHandlerMiddleware(ILogger<ConsumerErrorHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public override async Task Handle<TMessage>(IConsumeContext<TMessage> context, CancellationToken token)
    {
        try
        {
            await Next(context, token);
            _logger.LogInformation("Messages successfully received and committed from topic: {Topic}, last offset {Offset} from {Partition} partition",
                context.TopicName,context.Offset,context.Partition);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error while consuming messages from kafka");
            throw;
        }
    }
}