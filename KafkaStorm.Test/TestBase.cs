using Confluent.Kafka;
using KafkaStorm.Extensions;
using KafkaStorm.Interfaces;
using KafkaStorm.Test.TestConsumers;
using KafkaStorm.Test.TestEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace KafkaStorm.Test;

public abstract class TestBase
{
    private readonly IServiceScopeFactory _scopeFactory;

    protected TestBase()
    {
        var services = ConfigureServices();
        _scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
    }

    private IServiceCollection ConfigureServices()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton(Mock.Of<IHostEnvironment>(w =>
            w.EnvironmentName == "Development" && w.ApplicationName == "KafkaStorm"));
        collection.AddKafkaStorm(factory =>
        {
            factory.AddProducer(prf =>
            {
                prf.ConfigProducer(new ProducerConfig {BootstrapServers = "localhost:29092",});

                prf.InMemoryQueue();

                prf.SetQueueLimit(65536);
            });

            factory.StartProducerHostedService();

            factory.AddConsumers(crf =>
            {
                crf.SetConsumingPeriod(5);
                
                crf.AddConsumer<HelloConsumer, HelloEvent>(
                    new ConsumerConfig {BootstrapServers = "localhost:29092", GroupId = "TestGroup"}, "my-topic");
            });
        });
        return collection;
    }

    protected async Task Produce<TMessage>(TMessage message)
    {
        using var scope = _scopeFactory.CreateScope();
        var producer = scope.ServiceProvider.GetRequiredService<IProducer>();
        await producer.ProduceNowAsync(message, "my-topic");
    }
}