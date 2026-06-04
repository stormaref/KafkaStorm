using KafkaStorm.Interfaces;
using KafkaStorm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Registration;

public class KafkaStormRegistrationFactory(IServiceCollection serviceCollection)
{
    public void StartProducerHostedService()
    {
        serviceCollection.AddSingleton<IMessageStore, MessageStore>();
        serviceCollection.AddHostedService<ProducerHostedService>();
    }

    public void AddProducer(Action<ProducerRegistrationFactory> configure) =>
        configure(new ProducerRegistrationFactory(serviceCollection));

    public void AddConsumers(Action<ConsumerRegistrationFactory> configure) =>
        configure(new ConsumerRegistrationFactory(serviceCollection));
}
