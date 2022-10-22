using Confluent.Kafka;

namespace DKZKV.Kafka.Settings;

/// <summary>
/// Settings for kafka connection
/// </summary>
public class KafkaConnectionSettings 
{
    /// <summary>
    /// List of broker servers ({host}:{port})
    /// </summary>
    public string Servers { get; set; }

    /// <summary>
    /// Security protocol (default No)
    /// </summary>
    public SecurityProtocol? SecurityProtocol { get; set; } = null;

    /// <summary>
    /// Authentication mechanism  
    /// </summary>
    public SaslMechanism? SaslMechanism { get; set; } = null;

    /// <summary>
    /// Auth user name
    /// </summary>
    public string SaslUsername { get; set; } = null;

    /// <summary>
    /// Auth password
    /// </summary>
    public string SaslPassword { get; set; } = null;
}