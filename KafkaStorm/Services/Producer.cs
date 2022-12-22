using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaStorm.Extensions;
using KafkaStorm.Interfaces;
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
                                                      throw new Exception("Producer Config is null")).Build();
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

            try
            {
                await ProduceNowAsync(message, topicName);
            }
            catch (Exception e)
            {
                _messageStore.AddMessage(message, topicName);
            }
        });

        return Task.CompletedTask;
    }

    public async Task ProduceNowAsync<TMessage>(TMessage message, string? topicName = null)
    {
        var topic = topicName ?? typeof(TMessage).Name;
        var dr = await _producer.ProduceAsync(topic, new Message<Null, string>
        {
            Value = message.ToJsonString()
        });
        _producer.Flush();
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}