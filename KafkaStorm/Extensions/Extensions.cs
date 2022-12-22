using System;
using KafkaStorm.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStorm.Extensions;

public static class Extensions
{
    public static void AddKafkaStorm(this IServiceCollection collection, Action<KafkaStormRegistrationFactory> ksrf)
    {
        ksrf.Invoke(new KafkaStormRegistrationFactory(collection));
    }
}