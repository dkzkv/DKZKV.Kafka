using System.Text;
using DKZKV.Kafka.Abstractions;
using DKZKV.Kafka.Abstractions.Producer;
using DKZKV.Kafka.Exceptions;

namespace DKZKV.Kafka.Settings;

internal class KafkaGlobalProducerSettings : IKafkaGlobalProducerSettings
{
    private const int OptimalBufferingMessagesCount = 100_000;
    public TimeSpan ProduceTimeout { get; set; } = TimeSpan.Zero;
    public MessageCompression Compression { get; set; } = MessageCompression.None;
    public int QueueBufferingMaxMessages { get; set; } = OptimalBufferingMessagesCount;

    public void Validate()
    {
        var errors = new StringBuilder();
        
        if (QueueBufferingMaxMessages < OptimalBufferingMessagesCount)
            errors.AppendLine($"'{nameof(QueueBufferingMaxMessages)}' should be greater than {OptimalBufferingMessagesCount}, otherwise kafka will work not effective");
            
        if (errors.Length > 0)
            throw new ConsumerSettingsException(errors.ToString());
    }
}