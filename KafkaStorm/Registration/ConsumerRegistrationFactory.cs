using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Consumers.Registration;

public class ConsumerRegistrationFactory
{
    public IServiceCollection ServiceCollection;
    private ConsumerConfig _config;

    public ConsumerRegistrationFactory(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }

    public void SetConfig(ConsumerConfig config) =>
        _config = config;
}