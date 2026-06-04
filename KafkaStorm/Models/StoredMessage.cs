using KafkaStorm.Exceptions;

namespace KafkaStorm.Models;

public sealed class StoredMessage(string topic, object body)
{
    public string Topic { get; } = topic;
    public object Body { get; } = body;

    public static StoredMessage Create<T>(T message, string? topicName = null)
    {
        if (message is null)
            throw new MessageNullException<T>();

        var topic = string.IsNullOrWhiteSpace(topicName) ? typeof(T).Name : topicName;
        return new StoredMessage(topic, message);
    }
}
