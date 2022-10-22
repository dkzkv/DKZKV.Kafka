namespace DKZKV.Kafka.Abstractions.Producer;

/// <summary>
/// Global settings for all producers
/// </summary>
public interface IKafkaGlobalProducerSettings
{
    /// <summary>
    /// Time interval for sending message batch
    /// (Default 0) infinite wait
    /// </summary>
    public TimeSpan ProduceTimeout { get; set; }
        
    /// <summary>
    /// Compression type
    /// (Default None)
    /// </summary>
    public MessageCompression Compression { get; set; }
        
    /// <summary>
    /// Max common producers messages capacity
    /// (Should be greater than 100K) 
    /// </summary>
    public int QueueBufferingMaxMessages { get; set; }
}