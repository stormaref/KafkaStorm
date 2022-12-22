using System;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaStorm.Exceptions;
using KafkaStorm.Interfaces;
using KafkaStorm.Models;
using KafkaStorm.Registration;

namespace KafkaStorm.Services;

public class Producer : IProducer
{
    private readonly IMessageStore _messageStore;
    private readonly IProducer<Null, string> _producer;

    public Producer(IMessageStore messageStore)
    {
        _messageStore = messageStore;
        _producer = new ProducerBuilder<Null, string>(ProducerRegistrationFactory.ProducerConfig ??
                                                      throw new ProducerConfigNullException()).Build();
    }

    public Task Produce<TMessage>(TMessage message, string? topicName = null)
    {
        Task.Run(async () =>
        {
            if (!ProducerRegistrationFactory.UseInMemoryQueue)
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
        if (message is null)
        {
            throw new ArgumentNullException(typeof(TMessage).Name);
        }

        var topic = topicName ?? typeof(TMessage).Name;
        var dr = await _producer.ProduceAsync(topic, new Message<Null, string>
        {
            Value = JsonSerializer.Serialize(message)
        });
        _producer.Flush();
    }

    public async Task ProduceNowAsync(StoredMessage message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var dr = await _producer.ProduceAsync(message.Topic, new Message<Null, string>
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
}