using Confluent.Kafka;

namespace KafkaStorm.Test.Helpers;

internal static class TestKafkaConfig
{
    internal const string BootstrapServers = "localhost:29092";
    internal const string TestGroupId = "KafkaStorm-TestGroup";

    internal static ProducerConfig CreateProducerConfig() => new()
    {
        BootstrapServers = BootstrapServers
    };

    internal static ConsumerConfig CreateConsumerConfig() => new()
    {
        BootstrapServers = BootstrapServers,
        GroupId = TestGroupId
    };
}
