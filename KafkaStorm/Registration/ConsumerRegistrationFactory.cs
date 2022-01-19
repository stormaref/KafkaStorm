using System.Collections.Generic;
using Confluent.Kafka;
using KafkaStorm.Exceptions;
using KafkaStorm.Interfaces;
using KafkaStorm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Registration;

public class ConsumerRegistrationFactory
{
    private readonly IServiceCollection _serviceCollection;
    internal static Dictionary<string, ConsumerConfig> ConsumerConfigs;

    public ConsumerRegistrationFactory(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
        ConsumerConfigs = new Dictionary<string, ConsumerConfig>();
    }
    
    /// <summary>
    /// Add consumer to kafka
    /// </summary>
    /// <param name="config"></param>
    /// <typeparam name="TConsumer">Type of your message consumer</typeparam>
    /// <typeparam name="TMessage">Type of your message (should be consumed by the passed consumer)</typeparam>
    public void AddConsumer<TConsumer, TMessage>(ConsumerConfig config)
        where TMessage : class
        where TConsumer : class, IConsumer<TMessage>
    {
        var succeeded = ConsumerConfigs.TryAdd(typeof(TConsumer).FullName, config);
        if (!succeeded)
        {
            throw new DuplicateConsumerException();
        }

        _serviceCollection.AddTransient<IConsumer<TMessage>, TConsumer>();
        _serviceCollection.AddHostedService<ConsumerHostedService<TMessage>>();
    }
}