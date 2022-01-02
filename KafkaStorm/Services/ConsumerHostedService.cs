using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaStorm.Consumers.Registration;
using KafkaStorm.Extensions;
using KafkaStorm.Interfaces;
using Microsoft.Extensions.Hosting;

namespace KafkaStorm.Services;

public class ConsumerHostedService<TMessage> : IHostedService, IDisposable where TMessage : class
{
    private readonly IConsumer<TMessage> _myConsumer;
    private readonly IConsumer<Ignore, string> _consumer;

    public ConsumerHostedService(IConsumer<TMessage> myConsumer)
    {
        _myConsumer = myConsumer;
        _consumer = new ConsumerBuilder<Ignore, string>(ConsumerRegistrationFactory.ConsumerConfig ??
                                                        throw new Exception("Consumer config is empty")).Build();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            _consumer.Subscribe(typeof(TMessage).Name);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(cancellationToken);
                    var message = result.Message.Value.DeserializeJson<TMessage>();
                    _myConsumer.Handle(message, cancellationToken);
                }
                catch (Exception e)
                    when (e is ArgumentNullException or JsonException or NotSupportedException)
                {
                    throw new JsonException("Error parsing json!");
                }
                catch (Exception e)
                {
                    throw new Exception("Unhandled exception");
                }
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