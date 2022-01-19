using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using KafkaStorm.Interfaces;
using KafkaStorm.Models;
using KafkaStorm.Registration;

namespace KafkaStorm.Services;

public class MessageStore : IMessageStore
{
    private readonly ConcurrentDictionary<Guid, Message> _dictionary;

    public MessageStore()
    {
        _dictionary = new();
    }

    public (bool Any, Guid Id, Message Message) GetLastMessage()
    {
        if (!_dictionary.Any())
        {
            return (false, Guid.Empty, default(Message));
        }

        var (key, value) = _dictionary.Last();
        return (true, key, value);
    }

    public bool RemoveMessage(Guid id)
    {
        _dictionary.TryRemove(id, out var message);
        return message != null;
    }

    public Guid AddMessage<TMessage>(TMessage message)
    {
        if (KafkaStormRegistrationFactory.LimitQueue && _dictionary.Count >= KafkaStormRegistrationFactory.MaximumQueueMessageCount)
        {
            RemoveFirstMessage();
        }

        var id = Guid.NewGuid();
        _dictionary.TryAdd(id, Message.Create(message));
        return id;
    }

    private void RemoveFirstMessage()
    {
        if (!RemoveMessage(_dictionary.First().Key))
        {
            throw new NotImplementedException();
        }
    }
}