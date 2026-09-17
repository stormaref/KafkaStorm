using KafkaStorm.Configuration;
using KafkaStorm.Interfaces;
using KafkaStorm.Models;

namespace KafkaStorm.Services;

/// <summary>
/// In-memory retry queue for messages that failed to reach Kafka.
/// </summary>
/// <remarks>
/// Ordering is strict FIFO: <see cref="GetLastMessage"/> returns the oldest pending message so
/// retries preserve the order in which they were produced, and the oldest message is the one
/// evicted when <see cref="ProducerOptions.MaximumQueueMessageCount"/> is reached. A linked list
/// paired with an index keeps every operation O(1), since the producer polls the head in a loop.
/// </remarks>
public sealed class MessageStore(ProducerOptions options) : IMessageStore
{
    private readonly Lock _gate = new();
    private readonly LinkedList<(Guid Id, StoredMessage Message)> _queue = new();
    private readonly Dictionary<Guid, LinkedListNode<(Guid Id, StoredMessage Message)>> _index = new();

    public (Guid id, StoredMessage? message) GetLastMessage()
    {
        lock (_gate)
        {
            var head = _queue.First;
            return head is null ? (Guid.Empty, null) : (head.Value.Id, head.Value.Message);
        }
    }

    public bool RemoveMessage(Guid id)
    {
        lock (_gate)
        {
            if (!_index.Remove(id, out var node))
                return false;

            _queue.Remove(node);
            return true;
        }
    }

    public Guid AddMessage<TMessage>(TMessage message, string? topicName = null)
    {
        // Built outside the lock so a null message throws before it can evict anything.
        var stored = StoredMessage.Create(message, topicName);
        var id = Guid.NewGuid();

        lock (_gate)
        {
            if (options.LimitQueue)
            {
                while (_queue.Count >= options.MaximumQueueMessageCount)
                    RemoveOldest();
            }

            _index[id] = _queue.AddLast((id, stored));
        }

        return id;
    }

    private void RemoveOldest()
    {
        var oldest = _queue.First ?? throw new InvalidOperationException("Max size should be more than 1");

        _queue.RemoveFirst();
        _index.Remove(oldest.Value.Id);
    }
}
