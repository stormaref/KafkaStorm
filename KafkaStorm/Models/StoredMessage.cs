using KafkaStorm.Exceptions;

namespace KafkaStorm.Models;

public class StoredMessage
{
    public StoredMessage(string topic, object body)
    {
        Topic = topic;
        Body = body;
    }

    public string Topic { get; private set; }
    public object Body { get; private set; }

    public static StoredMessage Create<T>(T message, string? topicName = null)
    {
        var topic = string.IsNullOrWhiteSpace(topicName) ? typeof(T).Name : topicName;
        if (message is null)
        {
            throw new MessageNullException<T>();
        }

        return new StoredMessage(topic, message);
    }
}