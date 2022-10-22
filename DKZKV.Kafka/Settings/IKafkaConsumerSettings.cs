using DKZKV.Kafka.Abstractions;

namespace DKZKV.Kafka.Settings;

/// <summary>
/// Extension for consumer setup
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface IKafkaConsumerSettings<TMessage> where TMessage : class, IKafkaMessage
{
    /// <summary>
    /// Topic name
    /// </summary>
    public string TopicName { get; set; }
        
    /// <summary>
    /// Name of consumer group
    /// </summary>
    public string GroupId { get; set; }
        
    /// <summary>
    /// Batch size
    /// </summary>
    public int BatchSize { get; set; }

    /// <summary>
    /// Consumers count, should be equal for partitions count. If not horizontal scale is not available
    /// (Default = 1)
    /// </summary>
    public int ConsumersCount { get; set; }
        
    /// <summary>
    /// Interval between end of partition and next reading
    /// (Default = 1000)
    /// </summary>
    public int EofConsumeDelayInMls { get; set; }
}