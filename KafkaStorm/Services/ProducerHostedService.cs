using System;
using System.Threading;
using System.Threading.Tasks;
using KafkaStorm.Interfaces;
using Microsoft.Extensions.Hosting;

namespace KafkaStorm.Services;

public class ProducerHostedService : IHostedService
{
    private readonly IProducer _producer;
    private readonly IMessageStore _messageStore;

    public ProducerHostedService(IProducer producer, IMessageStore messageStore)
    {
        _producer = producer;
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
            await _producer.ProduceNow(message);
            _messageStore.RemoveMessage(id);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _producer.Dispose();
        return Task.CompletedTask;
    }
}