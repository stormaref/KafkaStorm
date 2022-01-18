using System;
using System.Threading;
using System.Threading.Tasks;
using KafkaStorm.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaStorm.Services;

public class ProducerHostedService : IHostedService
{
    private readonly IServiceProvider _provider;
    private readonly IMessageStore _messageStore;

    public ProducerHostedService(IServiceProvider provider, IMessageStore messageStore)
    {
        _provider = provider;
        _messageStore = messageStore;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var (id, message) = _messageStore.GetLastMessage();
                if (id == Guid.Empty)
                {
                    continue;
                }

                Task.Run(async () => { await ProduceMessage(id, message); }, cancellationToken);
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    private async Task ProduceMessage(Guid id, object message)
    {
        try
        {
            using (var scope = _provider.CreateScope())
            {
                var _producer = scope.ServiceProvider.GetRequiredService<IProducer>();
                await _producer.ProduceNow(message);
            }
            _messageStore.RemoveMessage(id);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}