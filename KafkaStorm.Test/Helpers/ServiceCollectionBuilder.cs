using System.Reflection;
using Confluent.Kafka;
using KafkaStorm.Extensions;
using KafkaStorm.Registration;
using KafkaStorm.Test.TestConsumers;
using KafkaStorm.Test.TestEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace KafkaStorm.Test.Helpers;

internal sealed class ServiceCollectionBuilder
{
    private bool _withProducer;
    private bool _withProducerHostedService;
    private bool _withConsumers;
    private bool _withAssemblyScanning;
    private Action<ProducerRegistrationFactory>? _configureProducer;
    private Action<ConsumerRegistrationFactory>? _configureConsumers;

    internal ServiceCollectionBuilder WithProducer(Action<ProducerRegistrationFactory>? configure = null)
    {
        _withProducer = true;
        _configureProducer = configure;
        return this;
    }

    internal ServiceCollectionBuilder WithProducerHostedService()
    {
        _withProducerHostedService = true;
        return this;
    }

    internal ServiceCollectionBuilder WithConsumers(Action<ConsumerRegistrationFactory>? configure = null)
    {
        _withConsumers = true;
        _configureConsumers = configure;
        return this;
    }

    internal ServiceCollectionBuilder WithAssemblyScanning()
    {
        _withAssemblyScanning = true;
        return this;
    }

    internal ServiceCollectionBuilder WithDefaults()
    {
        return WithProducer(prf =>
            {
                prf.ConfigProducer(TestKafkaConfig.CreateProducerConfig());
                prf.InMemoryQueue();
                prf.SetQueueLimit(65536);
            })
            .WithProducerHostedService()
            .WithConsumers(crf =>
            {
                crf.SetConsumingPeriod(5);
                crf.AddConsumer<HelloConsumer, HelloEvent>(
                    TestKafkaConfig.CreateConsumerConfig(),
                    "my-topic");
            })
            .WithAssemblyScanning();
    }

    internal IServiceCollection Build()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Mock.Of<IHostEnvironment>(w =>
            w.EnvironmentName == "Development" && w.ApplicationName == "KafkaStorm"));

        services.AddKafkaStorm(factory =>
        {
            if (_withProducer)
            {
                factory.AddProducer(prf =>
                {
                    _configureProducer?.Invoke(prf);
                });
            }

            if (_withProducerHostedService)
                factory.StartProducerHostedService();

            if (_withConsumers || _withAssemblyScanning)
            {
                factory.AddConsumers(crf =>
                {
                    _configureConsumers?.Invoke(crf);

                    if (_withAssemblyScanning)
                    {
                        crf.AddConsumersFromAssembly(
                            Assembly.GetExecutingAssembly(),
                            TestKafkaConfig.CreateConsumerConfig());
                    }
                });
            }
        });

        return services;
    }

    internal ServiceProvider BuildProvider() => Build().BuildServiceProvider();
}
