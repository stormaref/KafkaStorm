namespace KafkaStorm.Models;

public class Message
{
    private Message(object messageObject, string type)
    {
        MessageObject = messageObject;
        Type = type;
    }

    public object MessageObject { get; }
    public string Type { get; }

    public static Message Create<TMessage>(TMessage message)
    {
        return new Message(message, typeof(TMessage).Name);
    }
}