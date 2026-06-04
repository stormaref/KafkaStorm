using KafkaStorm.Models;

namespace KafkaStorm.Interfaces;

public interface IProducer : IDisposable
{
    Task Produce<TMessage>(TMessage message, string? topicName = null);

    Task ProduceNowAsync<TMessage>(TMessage message, string? topicName = null);

    internal Task ProduceNowAsync(StoredMessage message);
}
