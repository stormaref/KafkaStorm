using FluentAssertions;
using KafkaStorm.Exceptions;

namespace KafkaStorm.Test.Unit.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void DuplicateConsumerException_ContainsConsumerName()
    {
        new DuplicateConsumerException("MyConsumer").Message.Should().Contain("MyConsumer");
    }

    [Fact]
    public void ConsumerNotConfiguredException_HasExpectedMessage()
    {
        new ConsumerNotConfiguredException().Message.Should().Be("Consumer is not configured");
    }

    [Fact]
    public void ProducerConfigNullException_HasExpectedMessage()
    {
        new ProducerConfigNullException().Message.Should().Be("Producer Config is null");
    }

    [Fact]
    public void MessageNullException_ContainsMessageTypeName()
    {
        new MessageNullException<string>().Message.Should().Contain(nameof(String));
    }
}
