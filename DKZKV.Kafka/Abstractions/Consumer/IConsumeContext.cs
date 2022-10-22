namespace DKZKV.Kafka.Abstractions.Consumer;

/// <summary>
/// Message with meta information
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface IConsumeContext<out TMessage> where TMessage : class, IKafkaMessage
{
    /// <summary>
    /// Topic name
    /// </summary>
    public string TopicName { get; }
    
    /// <summary>
    /// From witch partition number batch has been read      
    /// </summary>
    public int Partition { get; }
    
    /// <summary>
    /// Last batch offset
    /// </summary>
    public long Offset { get; }
    
    /// <summary>
    /// Message batch
    /// </summary>
    public IEnumerable<TMessage> Messages { get; }
}