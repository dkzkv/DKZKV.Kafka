using DKZKV.Kafka.Abstractions.Producer;
using JetBrains.Annotations;

namespace KafkaSampleService.ProducerMiddlewares;

[UsedImplicitly]
public class ErrorHandlerMiddleware : KafkaProducerMiddleware
{
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(ILogger<ErrorHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public override async Task Handle<TMessage>(TMessage[] messages, CancellationToken token)
    {
        _logger.LogDebug("Entered in {Middleware}, Layer 1", nameof(ErrorHandlerMiddleware));
        
        try
        {
            await Next(messages, token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error while sending message to kafka");
            throw;
        }
        
        _logger.LogDebug("Exited from {Middleware}, Layer 1", nameof(ErrorHandlerMiddleware));
    }
}