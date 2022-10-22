using System.Diagnostics;
using DKZKV.Kafka.Abstractions.Consumer;
using KafkaSampleService.Models;

namespace KafkaSampleService.Consumers;

public class TestMessageConsumer : IKafkaConsumer<TestMessage>
{
    private readonly ILogger<TestMessageConsumer> _logger;

    public TestMessageConsumer(ILogger<TestMessageConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(IConsumeContext<TestMessage> context, CancellationToken token)
    {
        foreach (var message in context.Messages)
        {
            Monitor.Do();
        }
        return Task.CompletedTask;
    }

    public static class Monitor
    {
        public static int Counter = 0;
        public static Stopwatch sw = new Stopwatch();
        public static void Do()
        {
            if (Counter == 0)
            {
                sw.Start();
            }
            else if (Counter >= 100000)
            {
                sw.Stop();
                var a = sw.Elapsed.TotalSeconds;
                ;
            }

            Counter++;
        }
    }
}