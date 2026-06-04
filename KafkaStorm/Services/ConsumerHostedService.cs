using System.Text.Json;
using Confluent.Kafka;
using KafkaStorm.Configuration;
using KafkaStorm.Exceptions;
using KafkaStorm.Interfaces;
using Microsoft.Extensions.Hosting;

namespace KafkaStorm.Services;

public sealed class ConsumerHostedService<TMessage>(
    IConsumer<TMessage> consumer,
    IConsumerRegistrationRegistry registry,
    ConsumerHostingOptions hostingOptions) :
    BackgroundService,
    IAsyncDisposable
    where TMessage : class
{
    private readonly IConsumer<Ignore, string> _kafkaConsumer = BuildKafkaConsumer(consumer, registry);
    private readonly string _topicName = ResolveTopicName(consumer, registry);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _kafkaConsumer.Subscribe(_topicName);

        while (!stoppingToken.IsCancellationRequested)
        {
            await HandleNextMessageAsync(stoppingToken);
        }
    }

    private async Task HandleNextMessageAsync(CancellationToken cancellationToken)
    {
        var result = _kafkaConsumer.Consume(hostingOptions.ConsumingPeriodMs);
        if (result is null)
            return;

        var message = JsonSerializer.Deserialize<TMessage>(result.Message.Value) ??
                      throw new MessageNullException<TMessage>();

        await consumer.Handle(message, cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _kafkaConsumer.Close();
        return base.StopAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        _kafkaConsumer.Dispose();
        return ValueTask.CompletedTask;
    }

    private static IConsumer<Ignore, string> BuildKafkaConsumer(
        IConsumer<TMessage> consumer,
        IConsumerRegistrationRegistry registry)
    {
        var fullName = consumer.GetType().FullName;
        ArgumentException.ThrowIfNullOrEmpty(fullName);

        if (!registry.TryGetRegistration(fullName, out var registration))
            throw new ConsumerNotConfiguredException();

        return new ConsumerBuilder<Ignore, string>(registration.Config).Build();
    }

    private static string ResolveTopicName(IConsumer<TMessage> consumer, IConsumerRegistrationRegistry registry)
    {
        var fullName = consumer.GetType().FullName;
        ArgumentException.ThrowIfNullOrEmpty(fullName);

        if (!registry.TryGetRegistration(fullName, out var registration))
            throw new ConsumerNotConfiguredException();

        return registration.Topic;
    }
}
