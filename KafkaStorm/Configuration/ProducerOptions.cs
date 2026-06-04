using Confluent.Kafka;

namespace KafkaStorm.Configuration;

public sealed class ProducerOptions
{
    public ProducerConfig? Config { get; set; }
    public bool UseInMemoryQueue { get; set; } = true;
    public bool LimitQueue { get; set; }
    public uint MaximumQueueMessageCount { get; set; } = ushort.MaxValue;
}
