namespace KafkaStorm.Configuration;

public interface IConsumerRegistrationRegistry
{
    bool TryGetRegistration(string consumerFullName, out ConsumerRegistration registration);

    void Register(string consumerFullName, ConsumerRegistration registration);
}
