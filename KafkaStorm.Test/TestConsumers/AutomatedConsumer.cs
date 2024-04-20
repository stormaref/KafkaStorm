using KafkaStorm.Interfaces;
using KafkaStorm.Test.TestEvents;

namespace KafkaStorm.Test.TestConsumers;

public class AutomatedConsumer : IConsumer<AutomatedEvent>
{
    public Task Handle(AutomatedEvent message, CancellationToken cancellationToken)
    {
        Console.WriteLine(message.Title);
        return Task.CompletedTask;
    }
}