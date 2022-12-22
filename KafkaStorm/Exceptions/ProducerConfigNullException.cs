using System;

namespace KafkaStorm.Exceptions;

public class ProducerConfigNullException : Exception
{
    public ProducerConfigNullException() : base("Producer Config is null")
    {
    }
}