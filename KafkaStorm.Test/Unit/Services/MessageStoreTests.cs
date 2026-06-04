using FluentAssertions;
using KafkaStorm.Configuration;
using KafkaStorm.Services;
using KafkaStorm.Test.TestEvents;

namespace KafkaStorm.Test.Unit.Services;

public class MessageStoreTests
{
    [Fact]
    public void GetLastMessage_WhenEmpty_ReturnsEmptyGuidAndNull()
    {
        var store = CreateStore();

        var (id, message) = store.GetLastMessage();

        id.Should().Be(Guid.Empty);
        message.Should().BeNull();
    }

    [Fact]
    public void AddMessage_StoresMessageThatCanBeRetrieved()
    {
        var store = CreateStore();
        var helloEvent = new HelloEvent("test");

        var id = store.AddMessage(helloEvent, "my-topic");

        id.Should().NotBe(Guid.Empty);
        var (retrievedId, retrievedMessage) = store.GetLastMessage();
        retrievedId.Should().Be(id);
        retrievedMessage!.Topic.Should().Be("my-topic");
        retrievedMessage.Body.Should().Be(helloEvent);
    }

    [Fact]
    public void RemoveMessage_RemovesStoredMessage()
    {
        var store = CreateStore();
        var id = store.AddMessage(new HelloEvent("test"));

        store.RemoveMessage(id).Should().BeTrue();
        store.GetLastMessage().id.Should().Be(Guid.Empty);
    }

    [Fact]
    public void RemoveMessage_WhenIdUnknown_ReturnsFalse()
    {
        var store = CreateStore();

        store.RemoveMessage(Guid.NewGuid()).Should().BeFalse();
    }

    [Fact]
    public void AddMessage_WhenQueueLimitReached_KeepsAtMostMaximumQueueMessageCount()
    {
        var options = new ProducerOptions { LimitQueue = true, MaximumQueueMessageCount = 2 };
        var store = new MessageStore(options);

        var id1 = store.AddMessage(new HelloEvent("first"), "topic-1");
        var id2 = store.AddMessage(new HelloEvent("second"), "topic-2");
        var id3 = store.AddMessage(new HelloEvent("third"), "topic-3");

        // ConcurrentDictionary iteration order is undefined; verify the size invariant instead.
        var remainingCount = new[] { id1, id2, id3 }.Count(store.RemoveMessage);
        remainingCount.Should().Be(2);
    }

    private static MessageStore CreateStore() => new(new ProducerOptions());
}
