using System;

namespace KafkaStorm.Exceptions;

public class DuplicateConsumerException : Exception
{
    public DuplicateConsumerException() : base("Consumer added more than once")
    {
    }
}