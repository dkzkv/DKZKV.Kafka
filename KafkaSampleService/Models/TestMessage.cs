using DKZKV.Kafka.Abstractions;
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace KafkaSampleService.Models;

public class TestMessage : IKafkaMessage
{
    public string TestValue { get; set; }
}