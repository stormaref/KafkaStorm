using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaStorm.Extensions;

namespace KafkaStorm.Services;

public interface IProducer
{
    Task ProduceAsync<T>(T message);
}

public class Producer : IProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;

    public Producer(ProducerConfig config)
    {
        _producer = new ProducerBuilder<Null, string>(config).Build();
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