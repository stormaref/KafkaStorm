using FluentAssertions;
using KafkaStorm.Configuration;
using KafkaStorm.Interfaces;
using KafkaStorm.Test.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Test.Unit.Registration;

public class KafkaStormRegistrationTests
{
    [Fact]
    public void AddKafkaStorm_WithDefaults_ResolvesCoreServices()
    {
        using var provider = new ServiceCollectionBuilder().WithDefaults().BuildProvider();
        using var scope = provider.CreateScope();

        scope.ServiceProvider.GetService<IProducer>().Should().NotBeNull();
        provider.GetService<IMessageStore>().Should().NotBeNull();
        provider.GetService<IConsumerRegistrationRegistry>().Should().NotBeNull();
    }

    [Fact]
    public void AddKafkaStorm_WithDefaults_ConfiguresProducerOptions()
    {
        using var provider = new ServiceCollectionBuilder().WithDefaults().BuildProvider();

        var options = provider.GetRequiredService<ProducerOptions>();
        options.Config.Should().NotBeNull();
        options.UseInMemoryQueue.Should().BeTrue();
        options.MaximumQueueMessageCount.Should().Be(65536u);
    }

    [Fact]
    public void AddKafkaStorm_WithProducerOnly_DoesNotRegisterMessageStore()
    {
        var services = new ServiceCollectionBuilder()
            .WithProducer(prf => prf.ConfigProducer(TestKafkaConfig.CreateProducerConfig()))
            .Build();

        services.Should().Contain(d => d.ServiceType == typeof(IProducer));
        services.Should().NotContain(d => d.ServiceType == typeof(IMessageStore));
    }
}
