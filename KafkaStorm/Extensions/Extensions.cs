using System;
using System.Text.Json;
using Confluent.Kafka;
using KafkaStorm.Interfaces;
using KafkaStorm.Registration;
using KafkaStorm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Extensions;

public static class Extensions
{
    public static void AddKafkaStorm(this IServiceCollection collection, Action<KafkaStormRegistrationFactory> ksrf)
    {
        ksrf.Invoke(new KafkaStormRegistrationFactory(collection));
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