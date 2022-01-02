using System;
using System.Text.Json;
using KafkaStorm.Consumers.Registration;
using KafkaStorm.Interfaces;
using KafkaStorm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Extensions;

public static class Extensions
{
    public static void AddKafkaStorm(this IServiceCollection collection, Action<ConsumerRegistrationFactory> crf)
    {
        crf.Invoke(new ConsumerRegistrationFactory(collection));
        collection.AddScoped<IProducer, Producer>();
    }
    
    /// <summary>
    /// Add consumer to kafka
    /// </summary>
    /// <param name="registrationFactory"></param>
    /// <typeparam name="TConsumer">Type of your message consumer</typeparam>
    /// <typeparam name="TMessage">Type of your message (should be consumed by the passed consumer)</typeparam>
    public static void AddConsumer<TConsumer, TMessage>(this ConsumerRegistrationFactory registrationFactory)
        where TMessage : class
        where TConsumer : class, IConsumer<TMessage>
    {
        registrationFactory.ServiceCollection.AddTransient<IConsumer<TMessage>, TConsumer>();
        registrationFactory.ServiceCollection.AddHostedService<ConsumerHostedService<TMessage>>();
    }

    public static string ToJsonString(this object obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public static T DeserializeJson<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }
}