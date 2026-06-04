using KafkaStorm.Models;

namespace KafkaStorm.Interfaces;

public interface IMessageStore
{
    (Guid id, StoredMessage? message) GetLastMessage();

    bool RemoveMessage(Guid id);

    Guid AddMessage<TMessage>(TMessage message, string? topicName = null);
}
