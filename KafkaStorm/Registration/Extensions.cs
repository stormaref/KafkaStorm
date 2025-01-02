using System;
using System.Collections.Generic;
using System.Linq;
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

    private static List<Type> GetMessageTypes(Assembly assembly)
    {
        return assembly.GetTypes().Where(t =>
                t.IsClass &&
                t.GetInterfaces().Contains(typeof(IMessage)))
            .ToList();
    }

    public static void AddConsumersFromAssembly(this ConsumerRegistrationFactory crf, Assembly assembly,
        ConsumerConfig config)
    {
        var messageTypes = GetMessageTypes(assembly);
        var method = typeof(ConsumerRegistrationFactory).GetMethod("AddConsumer");
        var methodInfos = (from messageType in messageTypes
                let consumerType = GetConsumerType(assembly, messageType)
                where consumerType != null
                select method!.MakeGenericMethod(consumerType, messageType))
            .ToList();
        foreach (var generic in methodInfos)
        {
            generic.Invoke(crf, [config, null]);
        }
    }
}