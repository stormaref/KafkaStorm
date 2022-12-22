using System;
using KafkaStorm.Interfaces;
using KafkaStorm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Registration;

public class KafkaStormRegistrationFactory
{
    private readonly IServiceCollection _serviceCollection;

    public KafkaStormRegistrationFactory(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public void StartProducerHostedService()
    {
        _serviceCollection.AddSingleton<IMessageStore, MessageStore>();
        _serviceCollection.AddHostedService<ProducerHostedService>();
    }

    public void AddProducer(Action<ProducerRegistrationFactory> prf)
    {
        prf.Invoke(new ProducerRegistrationFactory(_serviceCollection));
    }

    public void AddConsumers(Action<ConsumerRegistrationFactory> crf)
    {
        crf.Invoke(new ConsumerRegistrationFactory(_serviceCollection));
    }
}