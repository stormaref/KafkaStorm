using KafkaStorm.Interfaces;
using KafkaStorm.Test.TestEvents;

namespace KafkaStorm.Test.TestConsumers;

public class HelloConsumer : IConsumer<HelloEvent>
{
    public Task Handle(HelloEvent message, CancellationToken cancellationToken)
    {
        Console.WriteLine(message.Body);
        return Task.CompletedTask;
    }
}