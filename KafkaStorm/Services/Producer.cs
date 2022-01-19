using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaStorm.Extensions;
using KafkaStorm.Interfaces;
using KafkaStorm.Registration;

namespace KafkaStorm.Services;

public class Producer : IProducer
{
    private readonly IProducer<Null, string> _producer;
    private readonly IMessageStore _messageStore;

    public Producer(IMessageStore messageStore)
    {
        _messageStore = messageStore;
        _producer = new ProducerBuilder<Null, string>(ProducerRegistrationFactory.ProducerConfig ??
                                                      throw new Exception("Producer Config is null")).Build();
    }

    public Task Produce<TMessage>(TMessage message)
    {
        Task.Run(async () =>
        {
            if (!ProducerRegistrationFactory.UseInMemoryQueue)
            {
                await ProduceNowAsync(message);
                return;
            }

            try
            {
                await ProduceNowAsync(message);
            }
            catch (Exception)
            {
                _messageStore.AddMessage(message);
            }
        });

        return Task.CompletedTask;
    }

    public async Task ProduceNowAsync<TMessage>(TMessage message, string? topicName = null)
    {
        string topic = topicName ?? typeof(TMessage).Name;
        var dr = await _producer.ProduceAsync(topic, new Message<Null, string>
        {
            Value = message.ToJsonString()
        });
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}