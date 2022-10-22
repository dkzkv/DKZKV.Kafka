# DKZK.Kafka
A library for integration with kafka, implementing a simplified wrapper over Confluent.Kafka
### Features
- The ability to override the serialization of messages
##### Producer
- Sending one messages
- Sending messages by batch, with a delivery guarantee
- Sending messages with a partitioning key
- Adding middleware for producer
##### Consumer
- Reading messages from kafka by batches (messages in batch, guaranteed from one partition)
- Adding middleware for the consumer
----
> **OFFTOP!**: View ./Samples with usage examples
### Producer setup
```csharp
public IServiceCollection services

services.AddKafka(conf =>
{
    // Override producer serializer
    //conf.SetProducerSerializer(new DefaultJsonSerializer());
    
    // Add producer for 'TestMessage' wich send messages to 'test-topic'       
    conf.AddProducer<TestMessage>("test-topic");
    
    // Add producer with partitioning key 
    conf.AddProducer<TestMessageWithKey>("test-topic-with-key", m => m.Key");
}, _configuration);
```
To add a producer to the project, you need to add `conf.AddProducer<{message type}>({topic name})`, where the message type is inherited from `IKafkaMessage`. If it is necessary to send a message with a partitioning key, then you need to specify the property of the message itself, which will be this key. In this case, the key can be composite, for example: `m=>$"{m.Key}:{m.AnotherKey}"`.
#### Example of using producer
```csharp
private readonly IKafkaProducer<TestMessage> _producer;
//TestMessage[] messages = GetMessages();
await _producer.Produce(messages, stoppingToken);
```
#### Extended settings for producer
```csharp
conf.SetGlobalProducersSettings(set =>
{
   set.Compression = MessageCompression.Snappy;
   set.ProduceTimeout = TimeSpan.FromSeconds(10);
   set.QueueBufferingMaxMessagesInMls = 1_000_000;
});
```
Settings definition:
- **Compression** - Compression for messaging (Default: None)
- **ProduceTimeout** - Interval of time for sending one batch (Default: 0, infinite waiting)
- **QueueBufferingMaxMessages** - Common maximum amount of messages for delivery in producers (Default: 100к and should not be less)>
  >**Warning!**: Change QueueBufferingMaxMessages only if you understand what this setting is responsible for in Confluent kafka. It may be required if a large batch size is sent at once, for example, more than 100k
#### Adding middleware for producer
```csharp
conf.AddProducerMiddleware<ErrorHandlerMiddleware>();
conf.AddProducerMiddleware<LoggingMiddleware>();
```
To log errors, metrics, etc., you can add middleware which the necessary functionality will be implemented. To do this, use `KafkaMiddleware`
>**Important**: middleware is called in the order in which they were added to DI

Producer middleware example:
```csharp
public class MyMiddleware : KafkaMiddleware
{
    public override async Task Handle<TMessage>(TMessage message)
    {
        //Before sending
        await Next(message);
        //After sending
    }
}
```
---
### Consumer setup
```csharp
services.AddKafka(conf =>
{
    conf.SetConsumersGroupId("foo");
    conf.AddConsumer<TestMessageConsumer, TestMessage>(TopicForTestMessages, BatchSize);
    conf.AddConsumer<TestMessageWithKeyConsumer, TestMessageWithKey>(set =>
    {
        set.TopicName = TopicForTestMessagesWithKey;
        set.GroupId = "foo1"; //override groupid for consumers
        set.BatchSize = BatchSize;
        set.ConsumersCount = 1; 
        set.EofConsumeDelayInMls = 1000;
    });
}, _configuration);
```
To add a consumer to the project, you need to add `conf.AddConsumer<{consumer type},{message type}>({topic name}, {Batch size})`. At the same time, the size of the batch only indicates that this is the maximum number of messages that the consumer can return. If the batch has reached the end of the partition, it may return less.

When adding consumers, you need to specify a common group `conf.SetConsumersGroupId("foo");`
>**Important!**: in case of a consumer serialization error, it stops reading. The contract should not be different
#### Consumer usage example
```csharp
public class TestMessageConsumer : IKafkaConsumer<TestMessage>
{
    private readonly ILogger<TestMessageConsumer> _logger;
    //ctor
    public Task Consume(IConsumeContext<TestMessage> context, CancellationToken token)
    {
        _logger.LogInformation("Messages received: {Count}", context.Messages.Count());
    }
}
```
`IConsumeContext` contains meta information on the incoming batch
- Topic Name
- Partition - partition number
- Offset - the number of the last offset in the partition
#### Adding middleware for consumer
```csharp
conf.AddConsumerMiddleware<ConsumerErrorHandlerMiddleware>();
```
---
#### Connectivity settings for kafka
The easiest way is to add the `KafkaConnectionSettings` section to appsettings.json, the settings will be automatically tightened. If you need more sophisticated ways to add settings for kafka, then use IConfigurationRoot to determine the settings of KafkaConnectionSettings
```json
  "KafkaConnectionSettings": {
    "Servers" : "localhost:9092",
    "SecurityProtocol" : Plaintext,
    "SaslMechanism" : Plain,
    "SaslUsername" : "test_user",
    "SaslPassword" : "test_user_password"
  }
```
If you need to connect without a password, then you do not need to fill in the fields responsible for connecting with a password
```json
  "KafkaConnectionSettings": {
"Servers" : "localhost:9092"
}
```
Or you can use extension method:
```csharp
services.AddKafka(conf =>
{
    //...
}, set =>
{
    set.Servers = "localhost:9092";
    ...
});
```