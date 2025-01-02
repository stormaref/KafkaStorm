using System;

namespace KafkaStorm.Exceptions;

public class MessageNullException<TMessage>()
    : ArgumentNullException("Message", $"Message of type {typeof(TMessage)} was null");