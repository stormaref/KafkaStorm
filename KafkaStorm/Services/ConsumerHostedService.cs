using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaStorm.Consumers.Extensions;
using KafkaStorm.Consumers.Interfaces;
using Microsoft.Extensions.Hosting;

namespace KafkaStorm.Consumers.Services;

public class ConsumerHostedService<TMessage> : IHostedService, IDisposable where TMessage : class
{
    private readonly IConsumer<TMessage> _myConsumer;
    private readonly IConsumer<Ignore, string> _consumer;

    public ConsumerHostedService(IConsumer<TMessage> myConsumer, ConsumerConfig consumerConfig)
    {
        _myConsumer = myConsumer;
        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            _consumer.Subscribe(typeof(TMessage).Name);
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(cancellationToken);
                var message = result.Message.Value.DeserializeJson<TMessage>();
                _myConsumer.Handle(message, cancellationToken);
            }
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Close();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }
}