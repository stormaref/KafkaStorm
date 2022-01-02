using Confluent.Kafka;
using KafkaStorm.Interfaces;
using KafkaStorm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Consumers.Registration;

public class ConsumerRegistrationFactory
{
    public IServiceCollection ServiceCollection;
    public static ConsumerConfig? ConsumerConfig;
    public static ProducerConfig? ProducerConfig;

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
}