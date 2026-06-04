using Confluent.Kafka;
using KafkaStorm.Configuration;
using KafkaStorm.Interfaces;
using KafkaStorm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Registration;

public class ProducerRegistrationFactory(IServiceCollection serviceCollection)
{
    private readonly ProducerOptions _producerOptions = GetOrCreateProducerOptions(serviceCollection);

    public void ConfigProducer(ProducerConfig config)
    {
        _producerOptions.Config = config;
        serviceCollection.AddScoped<IProducer, Producer>();
    }

    public void SetQueueLimit(uint count)
    {
        if (count == 0)
            throw new ArgumentOutOfRangeException(nameof(count), count, "Message count cannot be zero");

        _producerOptions.LimitQueue = true;
        _producerOptions.MaximumQueueMessageCount = count;
    }

    public void InMemoryQueue(bool activate = true) => _producerOptions.UseInMemoryQueue = activate;

    private static ProducerOptions GetOrCreateProducerOptions(IServiceCollection services)
    {
        foreach (var descriptor in services)
        {
            if (descriptor.ServiceType == typeof(ProducerOptions) &&
                descriptor.ImplementationInstance is ProducerOptions existing)
                return existing;
        }

        var options = new ProducerOptions();
        services.AddSingleton(options);
        return options;
    }
}
