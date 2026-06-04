using System.Collections.Concurrent;
using KafkaStorm.Configuration;
using KafkaStorm.Interfaces;
using KafkaStorm.Models;

namespace KafkaStorm.Services;

public sealed class MessageStore(ProducerOptions options) : IMessageStore
{
    private readonly ConcurrentDictionary<Guid, StoredMessage> _dictionary = new();
    private readonly ProducerOptions _options = options;

    public (Guid id, StoredMessage? message) GetLastMessage()
    {
        if (_dictionary.IsEmpty)
            return (Guid.Empty, null);

        var (key, value) = _dictionary.Last();
        return (key, value);
    }

    public bool RemoveMessage(Guid id)
    {
        return _dictionary.TryRemove(id, out _);
    }

    public Guid AddMessage<TMessage>(TMessage message, string? topicName = null)
    {
        if (_options.LimitQueue && _dictionary.Count >= _options.MaximumQueueMessageCount)
            RemoveFirstMessage();

        var id = Guid.NewGuid();
        _dictionary.TryAdd(id, StoredMessage.Create(message, topicName));
        return id;
    }

    private void RemoveFirstMessage()
    {
        if (!RemoveMessage(_dictionary.First().Key))
            throw new InvalidOperationException("Max size should be more than 1");
    }
}
