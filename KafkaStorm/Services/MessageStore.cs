using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using KafkaStorm.Interfaces;

namespace KafkaStorm.Services;

public class MessageStore : IMessageStore
{
    private readonly ConcurrentDictionary<Guid, object> _dictionary;

    public MessageStore()
    {
        _dictionary = new();
    }

    public (Guid Id, object Message) GetLastMessage()
    {
        if (!_dictionary.Any())
        {
            return (Guid.Empty, default(object));
        }
        var (key, value) = _dictionary.Last();
        return (key, value);
    }

    public bool RemoveMessage(Guid id)
    {
        _dictionary.TryRemove(id, out var message);
        return message != null;
    }

    public Guid AddMessage(object message)
    {
        var id = Guid.NewGuid();
        _dictionary.TryAdd(id, message);
        return id;
    }
}