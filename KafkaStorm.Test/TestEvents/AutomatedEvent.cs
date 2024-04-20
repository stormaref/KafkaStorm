using KafkaStorm.Interfaces;

namespace KafkaStorm.Test.TestEvents;

public class AutomatedEvent : IMessage
{
    public AutomatedEvent()
    {
        Title = "Automated";
    }
    public string Title { get; set; }
}