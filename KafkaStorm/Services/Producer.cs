using System.Text.Json;
using Confluent.Kafka;
using KafkaStorm.Configuration;
using KafkaStorm.Exceptions;
using KafkaStorm.Interfaces;
using KafkaStorm.Models;

namespace KafkaStorm.Services;

public sealed class Producer(ProducerOptions options, IMessageStore messageStore) : IProducer, IAsyncDisposable
{
    private readonly IMessageStore _messageStore = messageStore;
    private readonly IProducer<Null, string> _producer = new ProducerBuilder<Null, string>(
        options.Config ?? throw new ProducerConfigNullException()).Build();
    private readonly ProducerOptions _options = options;

    public Task Produce<TMessage>(TMessage message, string? topicName = null)
    {
        _ = Task.Run(async () =>
        {
            if (!_options.UseInMemoryQueue)
            {
                await ProduceNowAsync(message, topicName);
                return;
            }

            await ProduceWithQueue(message, topicName);
        });

        return Task.CompletedTask;
    }

    private async Task ProduceWithQueue<TMessage>(TMessage message, string? topicName)
    {
        try
        {
            await ProduceNowAsync(message, topicName);
        }
        catch (Exception)
        {
            _messageStore.AddMessage(message, topicName);
        }
    }

    public async Task ProduceNowAsync<TMessage>(TMessage message, string? topicName = null)
    {
        ArgumentNullException.ThrowIfNull(message);

        var topic = topicName ?? typeof(TMessage).Name;
        await _producer.ProduceAsync(topic, new Message<Null, string>
        {
            Value = JsonSerializer.Serialize(message)
        });
        _producer.Flush();
    }

    public async Task ProduceNowAsync(StoredMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        await _producer.ProduceAsync(message.Topic, new Message<Null, string>
        {
            Value = JsonSerializer.Serialize(message.Body)
        });
        _producer.Flush();
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
