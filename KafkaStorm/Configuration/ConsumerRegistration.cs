using Confluent.Kafka;

namespace KafkaStorm.Configuration;

public sealed record ConsumerRegistration(ConsumerConfig Config, string Topic);
