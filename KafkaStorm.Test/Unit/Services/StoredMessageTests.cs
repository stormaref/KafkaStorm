using FluentAssertions;
using KafkaStorm.Exceptions;
using KafkaStorm.Models;
using KafkaStorm.Test.TestEvents;

namespace KafkaStorm.Test.Unit.Services;

public class StoredMessageTests
{
    [Fact]
    public void Create_UsesMessageTypeNameAsDefaultTopic()
    {
        var message = new HelloEvent("hello");

        var stored = StoredMessage.Create(message);

        stored.Topic.Should().Be(nameof(HelloEvent));
        stored.Body.Should().Be(message);
    }

    [Fact]
    public void Create_UsesExplicitTopicWhenProvided()
    {
        var message = new HelloEvent("hello");

        var stored = StoredMessage.Create(message, "custom-topic");

        stored.Topic.Should().Be("custom-topic");
    }

    [Fact]
    public void Create_WhenMessageIsNull_ThrowsMessageNullException()
    {
        HelloEvent? message = null;

        var act = () => StoredMessage.Create(message!);

        act.Should().Throw<MessageNullException<HelloEvent>>();
    }
}
