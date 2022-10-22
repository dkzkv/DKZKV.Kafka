namespace DKZKV.Kafka.Exceptions;

internal class KafkaDeserializationExceptions : Exception
{
    public KafkaDeserializationExceptions (string topic, long offset, Exception inner) 
        : base($"Error while deserialization message from topic: {topic}, offset {offset}", inner)
    {}
}