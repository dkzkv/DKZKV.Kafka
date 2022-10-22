namespace DKZKV.Kafka.Abstractions;

/// <summary>
/// Message compression
/// </summary>
public enum MessageCompression
{
    /// <summary>
    /// None
    /// </summary>
    None,
    
    /// <summary>
    /// Gzip
    /// </summary>
    Gzip,
    
    /// <summary>
    /// Snappy
    /// </summary>
    Snappy
}