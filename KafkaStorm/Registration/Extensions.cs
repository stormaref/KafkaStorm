using System.Reflection;
using Confluent.Kafka;
using KafkaStorm.Interfaces;

namespace KafkaStorm.Registration;

public static class Extensions
{
    private static Type? GetConsumerType(Assembly assembly, Type messageType)
    {
        var iConsumerType = typeof(IConsumer<>).MakeGenericType(messageType);
        return assembly.GetTypes().FirstOrDefault(t =>
            t.GetInterfaces().Contains(iConsumerType) &&
            t is { IsClass: true, IsVisible: true });
    }

    private static List<Type> GetMessageTypes(Assembly assembly) =>
        assembly.GetTypes()
            .Where(t => t is { IsClass: true } && t.GetInterfaces().Contains(typeof(IMessage)))
            .ToList();

    public static void AddConsumersFromAssembly(
        this ConsumerRegistrationFactory crf,
        Assembly assembly,
        ConsumerConfig config)
    {
        var messageTypes = GetMessageTypes(assembly);
        var method = typeof(ConsumerRegistrationFactory).GetMethod(nameof(ConsumerRegistrationFactory.AddConsumer));

        foreach (var messageType in messageTypes)
        {
            var consumerType = GetConsumerType(assembly, messageType);
            if (consumerType is null)
                continue;

            var generic = method!.MakeGenericMethod(consumerType, messageType);
            generic.Invoke(crf, [config, null]);
        }
    }
}
