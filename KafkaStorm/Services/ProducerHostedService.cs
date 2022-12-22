using System;
using System.Threading;
using System.Threading.Tasks;
using KafkaStorm.Interfaces;
using KafkaStorm.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaStorm.Services;

public class ProducerHostedService : IHostedService
{
    private readonly IMessageStore _messageStore;
    private readonly IServiceProvider _provider;

    public ProducerHostedService(IServiceProvider provider, IMessageStore messageStore)
    {
        _provider = provider;
        _messageStore = messageStore;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var (id, message) = _messageStore.GetLastMessage();
                if (id == Guid.Empty) continue;

                await ProduceMessage(id, message ?? throw new ArgumentNullException(nameof(message)));
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task ProduceMessage(Guid id, StoredMessage storedMessage)
    {
        try
        {
            using var scope = _provider.CreateScope();
            var producer = scope.ServiceProvider.GetRequiredService<IProducer>();
            await producer.ProduceNowAsync(storedMessage);
            _messageStore.RemoveMessage(id);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}