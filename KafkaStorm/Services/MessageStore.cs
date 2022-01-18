using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using KafkaStorm.Interfaces;
using KafkaStorm.Models;

namespace KafkaStorm.Services;

public class MessageStore : IMessageStore
{
    private readonly ConcurrentDictionary<Guid, Message> _dictionary;

    public MessageStore()
    {
        _dictionary = new();
    }

    public (Guid Id, Message Message) GetLastMessage()
    {
        if (!_dictionary.Any())
        {
            return (Guid.Empty, default(Message));
        }

        var (key, value) = _dictionary.Last();
        return (key, value);
    }

    public bool RemoveMessage(Guid id)
    {
        _dictionary.TryRemove(id, out var message);
        return message != null;
    }

    public Guid AddMessage<TMessage>(TMessage message)
    {
        var id = Guid.NewGuid();
        _dictionary.TryAdd(id, Message.Create(message));
        return id;
    }
}