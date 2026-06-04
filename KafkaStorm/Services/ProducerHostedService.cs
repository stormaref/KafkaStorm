using KafkaStorm.Interfaces;
using KafkaStorm.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaStorm.Services;

public sealed class ProducerHostedService(
    IServiceProvider provider,
    IMessageStore messageStore,
    ILogger<ProducerHostedService> logger) : BackgroundService
{
    private static readonly TimeSpan EmptyQueueDelay = TimeSpan.FromMilliseconds(100);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var (id, message) = messageStore.GetLastMessage();
            if (id == Guid.Empty)
            {
                await Task.Delay(EmptyQueueDelay, stoppingToken);
                continue;
            }

            await ProduceMessageAsync(id, message ?? throw new ArgumentNullException(nameof(message)), stoppingToken);
        }
    }

    private async Task ProduceMessageAsync(Guid id, StoredMessage storedMessage, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = provider.CreateScope();
            var producer = scope.ServiceProvider.GetRequiredService<IProducer>();
            await producer.ProduceNowAsync(storedMessage);
            messageStore.RemoveMessage(id);
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogDebug(ex, "Failed to retry producing queued message {MessageId}", id);
        }
    }
}
