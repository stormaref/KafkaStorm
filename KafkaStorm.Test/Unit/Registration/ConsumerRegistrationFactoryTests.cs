using FluentAssertions;
using KafkaStorm.Configuration;
using KafkaStorm.Exceptions;
using KafkaStorm.Registration;
using KafkaStorm.Test.Helpers;
using KafkaStorm.Test.TestConsumers;
using KafkaStorm.Test.TestEvents;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Test.Unit.Registration;

public class ConsumerRegistrationFactoryTests
{
    [Fact]
    public void AddConsumer_RegistersConsumerInRegistryWithExplicitTopic()
    {
        var services = new ServiceCollection();
        var factory = new ConsumerRegistrationFactory(services);

        factory.AddConsumer<HelloConsumer, HelloEvent>(
            TestKafkaConfig.CreateConsumerConfig(),
            "explicit-topic");

        using var provider = services.BuildServiceProvider();
        var registry = provider.GetRequiredService<IConsumerRegistrationRegistry>();
        registry.TryGetRegistration(typeof(HelloConsumer).FullName!, out var registration).Should().BeTrue();
        registration.Topic.Should().Be("explicit-topic");
    }

    [Fact]
    public void AddConsumer_UsesMessageTypeNameAsDefaultTopic()
    {
        var services = new ServiceCollection();
        var factory = new ConsumerRegistrationFactory(services);

        factory.AddConsumer<HelloConsumer, HelloEvent>(TestKafkaConfig.CreateConsumerConfig());

        using var provider = services.BuildServiceProvider();
        var registry = provider.GetRequiredService<IConsumerRegistrationRegistry>();
        registry.TryGetRegistration(typeof(HelloConsumer).FullName!, out var registration).Should().BeTrue();
        registration.Topic.Should().Be(nameof(HelloEvent));
    }

    [Fact]
    public void AddConsumer_WhenDuplicate_ThrowsDuplicateConsumerException()
    {
        var services = new ServiceCollection();
        var factory = new ConsumerRegistrationFactory(services);
        var config = TestKafkaConfig.CreateConsumerConfig();

        factory.AddConsumer<HelloConsumer, HelloEvent>(config);

        var act = () => factory.AddConsumer<HelloConsumer, HelloEvent>(config);

        act.Should().Throw<DuplicateConsumerException>()
            .WithMessage("*HelloConsumer*");
    }

    [Fact]
    public void SetConsumingPeriod_UpdatesConsumerHostingOptions()
    {
        var services = new ServiceCollection();
        var factory = new ConsumerRegistrationFactory(services);

        factory.SetConsumingPeriod(25);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<ConsumerHostingOptions>().ConsumingPeriodMs.Should().Be(25);
    }
}
