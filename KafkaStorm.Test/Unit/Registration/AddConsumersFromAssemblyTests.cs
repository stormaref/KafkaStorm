using FluentAssertions;
using KafkaStorm.Configuration;
using KafkaStorm.Registration;
using KafkaStorm.Test.Helpers;
using KafkaStorm.Test.TestConsumers;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Test.Unit.Registration;

public class AddConsumersFromAssemblyTests
{
    [Fact]
    public void AddConsumersFromAssembly_DiscoversAutomatedConsumer()
    {
        var services = new ServiceCollection();
        var factory = new ConsumerRegistrationFactory(services);

        factory.AddConsumersFromAssembly(typeof(AutomatedConsumer).Assembly, TestKafkaConfig.CreateConsumerConfig());

        using var provider = services.BuildServiceProvider();
        var registry = provider.GetRequiredService<IConsumerRegistrationRegistry>();
        registry.TryGetRegistration(typeof(AutomatedConsumer).FullName!, out var registration).Should().BeTrue();
        registration.Topic.Should().Be("AutomatedEvent");
    }

    [Fact]
    public void AddConsumersFromAssembly_DoesNotRegisterHelloConsumerWithoutExplicitCall()
    {
        var services = new ServiceCollection();
        var factory = new ConsumerRegistrationFactory(services);

        factory.AddConsumersFromAssembly(typeof(AutomatedConsumer).Assembly, TestKafkaConfig.CreateConsumerConfig());

        using var provider = services.BuildServiceProvider();
        var registry = provider.GetRequiredService<IConsumerRegistrationRegistry>();
        registry.TryGetRegistration(typeof(HelloConsumer).FullName!, out _).Should().BeFalse();
    }
}
