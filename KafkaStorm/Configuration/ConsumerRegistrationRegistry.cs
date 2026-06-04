using System.Collections.Concurrent;

namespace KafkaStorm.Configuration;

public sealed class ConsumerRegistrationRegistry : IConsumerRegistrationRegistry
{
    private readonly ConcurrentDictionary<string, ConsumerRegistration> _registrations = new();

    public bool TryGetRegistration(string consumerFullName, out ConsumerRegistration registration) =>
        _registrations.TryGetValue(consumerFullName, out registration!);

    public void Register(string consumerFullName, ConsumerRegistration registration)
    {
        if (!_registrations.TryAdd(consumerFullName, registration))
            throw new InvalidOperationException($"Consumer '{consumerFullName}' is already registered.");
    }
}
