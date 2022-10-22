using DKZKV.Kafka.Abstractions.Producer;
using KafkaSampleService.Models;

namespace KafkaSampleService.Producers;

public class KafkaProducerTestMessagesWorker : BackgroundService
{
    private readonly IKafkaProducer<TestMessage> _producer;
    private readonly ILogger<KafkaProducerTestMessagesWorker> _logger;
    private const int GeneratedBatchSize = 1000;
    public KafkaProducerTestMessagesWorker(IKafkaProducer<TestMessage> producer,
        ILogger<KafkaProducerTestMessagesWorker> logger)
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

            if (batchCounter > 10000)
                break;
        }

        ;
    }

    private TestMessage[] GenerateBatchOfMessages(int batchCounter)
    {
        var messages = new TestMessage[GeneratedBatchSize];
        
        for (int i = 0; i < GeneratedBatchSize; i++)
        {
            messages[i] = new TestMessage()
            {
                TestValue = $"Test message: {batchCounter} | {_messageCounter}"
            };
            _messageCounter++;
        }
        return messages;
    }
    
}