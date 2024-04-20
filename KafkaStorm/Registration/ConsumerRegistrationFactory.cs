using System.Collections.Generic;
using Confluent.Kafka;
using KafkaStorm.Exceptions;
using KafkaStorm.Interfaces;
using KafkaStorm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Registration;

public class ConsumerRegistrationFactory
{
    public static Dictionary<string, ConsumerConfig> ConsumerConfigs = null!;
    internal static Dictionary<string, string> ConsumerTopics = null!;
    internal static int ConsumingPeriod = 10;
    private readonly IServiceCollection _serviceCollection;

    public ConsumerRegistrationFactory(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
        ConsumerConfigs = new Dictionary<string, ConsumerConfig>();
        ConsumerTopics = new Dictionary<string, string>();
    }

    /// <summary>
    ///     Add consumer to kafka
    /// </summary>
    /// <param name="config"></param>
    /// <param name="topicName">Name of topic</param>
    /// <typeparam name="TConsumer">Type of your message consumer</typeparam>
    /// <typeparam name="TMessage">Type of your message (should be consumed by the passed consumer)</typeparam>
    public void AddConsumer<TConsumer, TMessage>(ConsumerConfig config, string? topicName = null)
        where TMessage : class
        where TConsumer : class, IConsumer<TMessage>
    {
        var topic = string.IsNullOrWhiteSpace(topicName) ? typeof(TMessage).Name : topicName;

        var fullName = typeof(TConsumer).FullName!;
        var succeeded = ConsumerConfigs.TryAdd(fullName, config) &&
                        ConsumerTopics.TryAdd(fullName, topic);

        if (!succeeded) throw new DuplicateConsumerException(typeof(TConsumer).Name);

        _serviceCollection.AddTransient<IConsumer<TMessage>, TConsumer>();
        _serviceCollection.AddHostedService<ConsumerHostedService<TMessage>>();
    }

    /// <summary>
    /// Set consuming timeout (period for checking for new messages)
    /// </summary>
    /// <param name="period">period in milliseconds</param>
    public void SetConsumingPeriod(int period = 10)
    {
        ConsumingPeriod = period;
    }

}