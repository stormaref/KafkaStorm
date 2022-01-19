using System;
using System.Collections.Generic;
using Confluent.Kafka;
using KafkaStorm.Exceptions;
using KafkaStorm.Interfaces;
using KafkaStorm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Registration;

public class KafkaStormRegistrationFactory
{
    private readonly IServiceCollection _serviceCollection;
    public static ProducerConfig? ProducerConfig;
    public static bool UseInMemoryQueue = true;
    public static bool LimitQueue = false;
    public static uint MaximumQueueMessageCount = ushort.MaxValue;

    public KafkaStormRegistrationFactory(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public void AddProducer(ProducerConfig config)
    {
        ProducerConfig = config;
        _serviceCollection.AddScoped<IProducer, Producer>();
    }

    /// <summary>
    /// Limit queue to finite number of messages
    /// </summary>
    /// <param name="count">Number of messages</param>
    public void SetQueueLimit(uint count)
    {
        if (count == 0)
            throw new ArgumentOutOfRangeException(nameof(count), count, "Message count cannot be zero");

        LimitQueue = true;
        MaximumQueueMessageCount = count;
    }


    public void StartProducerHostedService()
    {
        _serviceCollection.AddSingleton<IMessageStore, MessageStore>();
        _serviceCollection.AddHostedService<ProducerHostedService>();
    }

    public void InMemoryQueue(bool activate = true)
    {
        UseInMemoryQueue = activate;
    }

    public void AddConsumers(Action<ConsumerRegistrationFactory> crf)
    {
        crf.Invoke(new ConsumerRegistrationFactory(_serviceCollection));
    }
}