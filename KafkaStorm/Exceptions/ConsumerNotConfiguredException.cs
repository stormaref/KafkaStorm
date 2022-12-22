using System;

namespace KafkaStorm.Exceptions;

public class ConsumerNotConfiguredException : Exception
{
    public ConsumerNotConfiguredException() : base("Consumer is not configured")
    {
    }
}