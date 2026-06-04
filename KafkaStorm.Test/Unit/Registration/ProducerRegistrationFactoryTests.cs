using FluentAssertions;
using KafkaStorm.Configuration;
using KafkaStorm.Interfaces;
using KafkaStorm.Registration;
using KafkaStorm.Test.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Test.Unit.Registration;

public class ProducerRegistrationFactoryTests
{
    [Fact]
    public void ConfigProducer_RegistersProducerOptionsAndProducer()
    {
        var services = new ServiceCollection();
        var factory = new ProducerRegistrationFactory(services);

        factory.ConfigProducer(TestKafkaConfig.CreateProducerConfig());

        services.Should().Contain(d => d.ServiceType == typeof(ProducerOptions));
        services.Should().Contain(d => d.ServiceType == typeof(IProducer));
    }

    [Fact]
    public void InMemoryQueue_TogglesUseInMemoryQueueOption()
    {
        var services = new ServiceCollection();
        var factory = new ProducerRegistrationFactory(services);
        factory.ConfigProducer(TestKafkaConfig.CreateProducerConfig());

        factory.InMemoryQueue(false);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<ProducerOptions>().UseInMemoryQueue.Should().BeFalse();
    }

    [Fact]
    public void SetQueueLimit_ConfiguresQueueLimitOptions()
    {
        var services = new ServiceCollection();
        var factory = new ProducerRegistrationFactory(services);
        factory.ConfigProducer(TestKafkaConfig.CreateProducerConfig());

        factory.SetQueueLimit(1024);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<ProducerOptions>();
        options.LimitQueue.Should().BeTrue();
        options.MaximumQueueMessageCount.Should().Be(1024u);
    }

    [Fact]
    public void SetQueueLimit_WhenZero_ThrowsArgumentOutOfRangeException()
    {
        var services = new ServiceCollection();
        var factory = new ProducerRegistrationFactory(services);

        var act = () => factory.SetQueueLimit(0);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
