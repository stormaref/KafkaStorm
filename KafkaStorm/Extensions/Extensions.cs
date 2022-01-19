using System;
using System.Text.Json;
using KafkaStorm.Registration;
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