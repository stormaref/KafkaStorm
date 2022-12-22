namespace KafkaStorm.Test.TestEvents;

public class HelloEvent
{
    public HelloEvent(string body)
    {
        Body = body;
    }

    public string Body { get; init; }
}