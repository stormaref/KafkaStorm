using Confluent.Kafka;
using KafkaStorm.Interfaces;
using KafkaStorm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Registration;

public class ConsumerRegistrationFactory
{
    public IServiceCollection ServiceCollection;
    public static ConsumerConfig? ConsumerConfig;
    public static ProducerConfig? ProducerConfig;
    public static bool UseInMemoryQueue = true;

    public ConsumerRegistrationFactory(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }

    public void SetConsumerConfig(ConsumerConfig config) =>
        ConsumerConfig = config;

    public void AddProducer(ProducerConfig config)
    {
        ProducerConfig = config;
        ServiceCollection.AddScoped<IProducer, Producer>();
    }

    public void StartProducerHostedService()
    {
        ServiceCollection.AddHostedService<ProducerHostedService>();
    }

    public void InMemoryQueue(bool activate = true)
    {
        UseInMemoryQueue = activate;
    }
}