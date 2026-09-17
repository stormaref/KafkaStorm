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
    private static readonly TimeSpan FailedRetryDelay = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var (id, message) = messageStore.GetLastMessage();
            if (message is null)
            {
                await Task.Delay(EmptyQueueDelay, stoppingToken);
                continue;
            }

            // The queue is FIFO, so a message that keeps failing stays at the head. Back off
            // instead of spinning on it.
            if (!await TryProduceMessageAsync(id, message, stoppingToken))
                await Task.Delay(FailedRetryDelay, stoppingToken);
        }
    }

    private async Task<bool> TryProduceMessageAsync(Guid id, StoredMessage storedMessage, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = provider.CreateScope();
            var producer = scope.ServiceProvider.GetRequiredService<IProducer>();
            await producer.ProduceNowAsync(storedMessage);
            messageStore.RemoveMessage(id);
            return true;
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogDebug(ex, "Failed to retry producing queued message {MessageId}", id);
            return false;
        }
    }
}
