using System;

namespace KafkaStorm.Exceptions;

public class DuplicateConsumerException : Exception
{
    public DuplicateConsumerException(string name) : base($"Consumer added more than once ({name})")
    {
    }
}