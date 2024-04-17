using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaStorm.Exceptions;
using KafkaStorm.Interfaces;
using KafkaStorm.Registration;
using Microsoft.Extensions.Hosting;

namespace KafkaStorm.Services;

public class ConsumerHostedService<TMessage> : IHostedService, IDisposable where TMessage : class
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IConsumer<TMessage> _myConsumer;
    private readonly string _topicName = null!;

    public ConsumerHostedService(IConsumer<TMessage> myConsumer)
    {
        _myConsumer = myConsumer;

        var fullName = _myConsumer.GetType().FullName;

        var succeeded =
            ConsumerRegistrationFactory.ConsumerConfigs.TryGetValue(fullName, out var config) &&
            ConsumerRegistrationFactory.ConsumerTopics.TryGetValue(fullName, out _topicName);

        _consumer = new ConsumerBuilder<Ignore, string>(succeeded
            ? config
            : throw new ConsumerNotConfiguredException()).Build();
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
            while (!cancellationToken.IsCancellationRequested) Handle(cancellationToken);
        }, cancellationToken);

        return Task.CompletedTask;
    }

    private void Handle(CancellationToken cancellationToken)
    {
        var result = _consumer.Consume(1);
        var message = JsonSerializer.Deserialize<TMessage>(result.Message.Value) ??
                      throw new MessageNullException<TMessage>();
        _myConsumer.Handle(message, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Close();
        return Task.CompletedTask;
    }
}