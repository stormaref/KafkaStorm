using System;
using Confluent.Kafka;
using KafkaStorm.Interfaces;
using KafkaStorm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Registration;

public class ProducerRegistrationFactory
{
    public static ProducerConfig? ProducerConfig;
    public static bool UseInMemoryQueue = true;
    public static bool LimitQueue;
    public static uint MaximumQueueMessageCount = ushort.MaxValue;
    private readonly IServiceCollection _serviceCollection;

    public ProducerRegistrationFactory(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public void ConfigProducer(ProducerConfig config)
    {
        ProducerConfig = config;
        _serviceCollection.AddScoped<IProducer, Producer>();
    }

    /// <summary>
    ///     Limit queue to finite number of messages
    /// </summary>
    /// <param name="count">Number of messages</param>
    public void SetQueueLimit(uint count)
    {
        if (count == 0)
            throw new ArgumentOutOfRangeException(nameof(count), count, "Message count cannot be zero");

        LimitQueue = true;
        MaximumQueueMessageCount = count;
    }

    public static void InMemoryQueue(bool activate = true)
    {
        UseInMemoryQueue = activate;
    }
}