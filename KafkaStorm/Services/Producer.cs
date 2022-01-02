using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaStorm.Consumers.Registration;
using KafkaStorm.Extensions;
using KafkaStorm.Interfaces;

namespace KafkaStorm.Services;

public class Producer : IProducer
{
    private readonly IProducer<Null, string> _producer;

    public Producer()
    {
        _producer = new ProducerBuilder<Null, string>(ConsumerRegistrationFactory.ProducerConfig ??
                                                      throw new Exception("Producer Config is null")).Build();
    }

    public async Task ProduceAsync<TMessage>(TMessage message)
    {
        var dr = await _producer.ProduceAsync(typeof(TMessage).Name, new Message<Null, string>
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