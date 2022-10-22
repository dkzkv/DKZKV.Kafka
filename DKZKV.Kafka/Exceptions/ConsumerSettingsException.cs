namespace DKZKV.Kafka.Exceptions;

/// <summary>
/// Invalid consumer configuration exception
/// </summary>
public class ConsumerSettingsException : Exception
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="message"></param>
    public ConsumerSettingsException (string message) 
        : base(message)
    {}
}