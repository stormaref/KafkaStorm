using Confluent.Kafka;
using KafkaStorm.Configuration;
using KafkaStorm.Exceptions;
using KafkaStorm.Interfaces;
using KafkaStorm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Registration;

public class ConsumerRegistrationFactory(IServiceCollection serviceCollection)
{
    private readonly IConsumerRegistrationRegistry _registry = GetOrCreateRegistry(serviceCollection);
    private readonly ConsumerHostingOptions _hostingOptions = GetOrCreateHostingOptions(serviceCollection);

    public void AddConsumer<TConsumer, TMessage>(ConsumerConfig config, string? topicName = null)
        where TMessage : class
        where TConsumer : class, IConsumer<TMessage>
    {
        var topic = string.IsNullOrWhiteSpace(topicName) ? typeof(TMessage).Name : topicName;
        var fullName = typeof(TConsumer).FullName!;

        try
        {
            _registry.Register(fullName, new ConsumerRegistration(config, topic));
        }
        catch (InvalidOperationException)
        {
            throw new DuplicateConsumerException(typeof(TConsumer).Name);
        }

        serviceCollection.AddTransient<IConsumer<TMessage>, TConsumer>();
        serviceCollection.AddHostedService<ConsumerHostedService<TMessage>>();
    }

    public void SetConsumingPeriod(int period = 10) => _hostingOptions.ConsumingPeriodMs = period;

    private static IConsumerRegistrationRegistry GetOrCreateRegistry(IServiceCollection services)
    {
        foreach (var descriptor in services)
        {
            if (descriptor.ServiceType == typeof(IConsumerRegistrationRegistry) &&
                descriptor.ImplementationInstance is IConsumerRegistrationRegistry existing)
                return existing;
        }

        var registry = new ConsumerRegistrationRegistry();
        services.AddSingleton<IConsumerRegistrationRegistry>(registry);
        return registry;
    }

    private static ConsumerHostingOptions GetOrCreateHostingOptions(IServiceCollection services)
    {
        foreach (var descriptor in services)
        {
            if (descriptor.ServiceType == typeof(ConsumerHostingOptions) &&
                descriptor.ImplementationInstance is ConsumerHostingOptions existing)
                return existing;
        }

        var options = new ConsumerHostingOptions();
        services.AddSingleton(options);
        return options;
    }
}
