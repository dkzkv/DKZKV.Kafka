namespace DKZKV.Kafka.Exceptions;

/// <summary>
/// Exception if no section or invalid connectivity settings
/// </summary>
public class KafkaSettingsExceptions : Exception
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="message"></param>
    public KafkaSettingsExceptions (string message) 
        : base(message)
    {}
}