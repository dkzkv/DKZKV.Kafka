using System.Text;
using DKZKV.Kafka.Abstractions;
using DKZKV.Kafka.Exceptions;

namespace DKZKV.Kafka.Settings
{
    internal class KafkaConsumerSettings<TMessage> : IKafkaConsumerSettings<TMessage> where TMessage : class, IKafkaMessage
    {
        public string TopicName { get; set; }
        public string GroupId { get; set; }
        public int BatchSize { get; set; }
        public int ConsumersCount { get; set; } = 1;
        public int EofConsumeDelayInMls { get; set; } = 1000;

        public void Validate()
        {
            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(TopicName))
                errors.AppendLine($"'{nameof(TopicName)}' should not be empty");
            if (string.IsNullOrEmpty(GroupId))
                errors.AppendLine($"'{nameof(GroupId)}' should not be empty");
            if (BatchSize <= 0)
                errors.AppendLine($"'{nameof(BatchSize)}' should be greater than zero");
            if (EofConsumeDelayInMls <= 0)
                errors.AppendLine($"'{nameof(EofConsumeDelayInMls)}' should be greater than zero");

            if (errors.Length > 0)
                throw new ConsumerSettingsException(errors.ToString());
        }
    }
}