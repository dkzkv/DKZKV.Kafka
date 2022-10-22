namespace DKZKV.Kafka.Exceptions;

/// <summary>
/// Producer exception
/// </summary>
public class KafkaProducerExceptions : Exception
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="message"></param>
    public KafkaProducerExceptions (string message) 
        : base(message)
    {}
}