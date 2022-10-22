using System.Diagnostics;
using DKZKV.Kafka.Abstractions.Producer;
using JetBrains.Annotations;

namespace KafkaSampleService.ProducerMiddlewares;

[UsedImplicitly]
public class LoggingMiddleware : KafkaProducerMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public override async Task Handle<TMessage>(TMessage[] messages, CancellationToken token)
    {
        _logger.LogDebug("Entered in {Middleware}, Layer 2", nameof(LoggingMiddleware));

        var duration = await MeasureDuration(async () =>
        {
            await Next(messages, token);
        });
        _logger.LogInformation("Messages was sent, duration: {DurationInMls} mls", duration);
        
        _logger.LogDebug("Exited from {Middleware}, Layer 2", nameof(LoggingMiddleware));
    }

    private async Task<long> MeasureDuration(Func<Task> action)
    {
        var sw = new Stopwatch();
        sw.Start();
        await action.Invoke();
        sw.Stop();
        return sw.ElapsedMilliseconds;
    }
}