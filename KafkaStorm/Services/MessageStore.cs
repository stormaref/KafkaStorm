using System;
using System.Collections.Concurrent;
using System.Linq;
using KafkaStorm.Interfaces;
using KafkaStorm.Models;
using KafkaStorm.Registration;

namespace KafkaStorm.Services;

public class MessageStore : IMessageStore
{
    private readonly ConcurrentDictionary<Guid, StoredMessage> _dictionary;

    public MessageStore()
    {
        _dictionary = new ConcurrentDictionary<Guid, StoredMessage>();
    }

    public (Guid id, StoredMessage? message) GetLastMessage()
    {
        if (!_dictionary.Any()) return (Guid.Empty, default);

        var (key, value) = _dictionary.Last();
        return (key, value);
    }

    public bool RemoveMessage(Guid id)
    {
        _dictionary.TryRemove(id, out var message);
        return message != null;
    }

    public Guid AddMessage<TMessage>(TMessage message, string? topicName = null)
    {
        if (ProducerRegistrationFactory.LimitQueue &&
            _dictionary.Count >= ProducerRegistrationFactory.MaximumQueueMessageCount)
            RemoveFirstMessage();

        var id = Guid.NewGuid();
        _dictionary.TryAdd(id, StoredMessage.Create(message, topicName));
        return id;
    }

    private void RemoveFirstMessage()
    {
        if (!RemoveMessage(_dictionary.First().Key)) throw new Exception("Max size should be more than 1");
    }
}