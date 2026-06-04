using Confluent.Kafka;
using FluentAssertions;
using KafkaStorm.Configuration;

namespace KafkaStorm.Test.Unit.Configuration;

public class ConsumerRegistrationRegistryTests
{
    [Fact]
    public void Register_AndTryGetRegistration_ReturnsRegistration()
    {
        var registry = new ConsumerRegistrationRegistry();
        var config = new ConsumerConfig { BootstrapServers = "localhost:9092", GroupId = "g1" };
        var registration = new ConsumerRegistration(config, "my-topic");

        registry.Register("MyConsumer", registration);

        registry.TryGetRegistration("MyConsumer", out var result).Should().BeTrue();
        result.Topic.Should().Be("my-topic");
        result.Config.GroupId.Should().Be("g1");
    }

    [Fact]
    public void Register_WhenDuplicate_ThrowsInvalidOperationException()
    {
        var registry = new ConsumerRegistrationRegistry();
        var registration = new ConsumerRegistration(
            new ConsumerConfig { BootstrapServers = "localhost:9092", GroupId = "g1" },
            "topic");

        registry.Register("MyConsumer", registration);

        var act = () => registry.Register("MyConsumer", registration);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*MyConsumer*");
    }

    [Fact]
    public void TryGetRegistration_WhenUnknown_ReturnsFalse()
    {
        var registry = new ConsumerRegistrationRegistry();

        registry.TryGetRegistration("Unknown", out _).Should().BeFalse();
    }
}
