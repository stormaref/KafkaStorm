using FluentAssertions;
using KafkaStorm.Configuration;
using KafkaStorm.Exceptions;
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
    public void AddMessage_WhenQueueLimitReached_EvictsOldestMessage()
    {
        var options = new ProducerOptions { LimitQueue = true, MaximumQueueMessageCount = 2 };
        var store = new MessageStore(options);

        var id1 = store.AddMessage(new HelloEvent("first"), "topic-1");
        var id2 = store.AddMessage(new HelloEvent("second"), "topic-2");
        var id3 = store.AddMessage(new HelloEvent("third"), "topic-3");

        store.RemoveMessage(id1).Should().BeFalse("the oldest message is evicted at the limit");
        new[] { id2, id3 }.Count(store.RemoveMessage).Should().Be(2);
    }

    [Fact]
    public void GetLastMessage_ReturnsOldestPendingMessageFirst()
    {
        var store = CreateStore();

        var first = store.AddMessage(new HelloEvent("first"), "topic-1");
        var second = store.AddMessage(new HelloEvent("second"), "topic-2");

        store.GetLastMessage().id.Should().Be(first);

        store.RemoveMessage(first);
        store.GetLastMessage().id.Should().Be(second);
    }

    [Fact]
    public void GetLastMessage_IsStableUntilTheHeadIsRemoved()
    {
        var store = CreateStore();
        var first = store.AddMessage(new HelloEvent("first"), "topic-1");
        store.AddMessage(new HelloEvent("second"), "topic-2");

        // The producer peeks repeatedly while a retry is in flight; the head must not drift.
        store.GetLastMessage().id.Should().Be(first);
        store.GetLastMessage().id.Should().Be(first);
    }

    [Fact]
    public void RemoveMessage_FromTheMiddle_KeepsRemainingOrder()
    {
        var store = CreateStore();
        var first = store.AddMessage(new HelloEvent("first"), "topic-1");
        var second = store.AddMessage(new HelloEvent("second"), "topic-2");
        var third = store.AddMessage(new HelloEvent("third"), "topic-3");

        store.RemoveMessage(second).Should().BeTrue();

        store.GetLastMessage().id.Should().Be(first);
        store.RemoveMessage(first);
        store.GetLastMessage().id.Should().Be(third);
    }

    [Fact]
    public void AddMessage_WhenMessageIsNull_DoesNotEvictAnything()
    {
        var options = new ProducerOptions { LimitQueue = true, MaximumQueueMessageCount = 1 };
        var store = new MessageStore(options);
        var existing = store.AddMessage(new HelloEvent("keep me"), "topic-1");

        var act = () => store.AddMessage<HelloEvent>(null!, "topic-2");

        act.Should().Throw<MessageNullException<HelloEvent>>();
        store.GetLastMessage().id.Should().Be(existing);
    }

    [Fact]
    public async Task AddMessage_IsSafeUnderConcurrentWriters()
    {
        var store = CreateStore();

        var ids = await Task.WhenAll(Enumerable.Range(0, 100).Select(i =>
            Task.Run(() => store.AddMessage(new HelloEvent($"m{i}"), "topic"))));

        ids.Distinct().Should().HaveCount(100);
        ids.Count(store.RemoveMessage).Should().Be(100);
        store.GetLastMessage().message.Should().BeNull();
    }

    private static MessageStore CreateStore() => new(new ProducerOptions());
}
