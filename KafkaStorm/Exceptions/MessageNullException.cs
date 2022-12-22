using System;

namespace KafkaStorm.Exceptions;

public class MessageNullException<TMessage> : ArgumentNullException
{
    public MessageNullException() : base("Message", $"Message of type {typeof(TMessage)} was null")
    {
    }
}