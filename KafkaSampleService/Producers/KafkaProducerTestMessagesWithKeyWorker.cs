using DKZKV.Kafka.Abstractions.Producer;
using KafkaSampleService.Models;

namespace KafkaSampleService.Producers;

public class KafkaProducerTestMessagesWithKeyWorker : BackgroundService
{
    private readonly IKafkaProducer<TestMessageWithKey> _producer;
    private readonly ILogger<KafkaProducerTestMessagesWithKeyWorker> _logger;
    private const int GeneratedBatchSize = 1000;
    public KafkaProducerTestMessagesWithKeyWorker(IKafkaProducer<TestMessageWithKey> producer,
        ILogger<KafkaProducerTestMessagesWithKeyWorker> logger)
    {
        _producer = producer;
        _logger = logger;
    }
    
    private int _messageCounter;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int batchCounter = 0;
        var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            var messages = GenerateBatchOfMessages(batchCounter);
            
            _logger.LogInformation("Sending message with key, batch: {Batch}", batchCounter);
            await _producer.Produce(messages, stoppingToken);
            _logger.LogInformation("Message with key has been sent, batch: {Batch}", batchCounter);
            batchCounter++;
        }
    }

    private TestMessageWithKey[] GenerateBatchOfMessages(int batchCounter)
    {
        var messages = new TestMessageWithKey[GeneratedBatchSize];
        
        for (int i = 0; i < GeneratedBatchSize; i++)
        {
            messages[i] = new TestMessageWithKey()
            {
                KeyPartOne = _messageCounter.ToString(),
                KeyPartTwo = "Key",
                TestValue = $"Test message: {batchCounter} | {_messageCounter}"
            };
            _messageCounter++;
        }
        return messages;
    }
}