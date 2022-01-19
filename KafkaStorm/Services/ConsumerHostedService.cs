using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaStorm.Extensions;
using KafkaStorm.Interfaces;
using KafkaStorm.Registration;
using Microsoft.Extensions.Hosting;

namespace KafkaStorm.Services;

public class ConsumerHostedService<TMessage> : IHostedService, IDisposable where TMessage : class
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IConsumer<TMessage> _myConsumer;
    private readonly string _topicName;

    public ConsumerHostedService(IConsumer<TMessage> myConsumer)
    {
        _myConsumer = myConsumer;

        var fullName = _myConsumer.GetType().FullName;

        var succeeded =
            ConsumerRegistrationFactory.ConsumerConfigs.TryGetValue(fullName, out var config) &&
            ConsumerRegistrationFactory.ConsumerTopics.TryGetValue(fullName, out _topicName);

        _consumer = new ConsumerBuilder<Ignore, string>(succeeded
            ? config
            : throw new Exception("Consumer config is empty")).Build();
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            _consumer.Subscribe(_topicName);
            while (!cancellationToken.IsCancellationRequested)
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
                    when (e is ConsumeException)
                {
                    //TODO: log
                }
                catch (Exception e)
                {
                    throw new Exception("Unhandled exception");
                }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Close();
        return Task.CompletedTask;
    }
}