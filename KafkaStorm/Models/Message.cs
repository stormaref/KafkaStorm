namespace KafkaStorm.Models;

public class Message
{
    private Message(object topicName, string type)
    {
        topicName = topicName;
        Type = type;
    }

    public object topicName { get; }
    public string Type { get; }

    public static Message Create<TMessage>(TMessage message, string? topicName = null)
    {
        var topic = string.IsNullOrWhiteSpace(topicName) ? typeof(TMessage).Name : topicName;
        return new Message(message, topic);
    }
}