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
    private readonly IServiceProvider _provider;
    private readonly IMessageStore _messageStore;

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
                var (any, id, message) = _messageStore.GetLastMessage();
                if (!any)
                {
                    continue;
                }

                await ProduceMessage(id, message);
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    private async Task ProduceMessage(Guid id, Message message)
    {
        try
        {
            using (var scope = _provider.CreateScope())
            {
                var _producer = scope.ServiceProvider.GetRequiredService<IProducer>();
                await _producer.ProduceNowAsync(message.MessageObject, message.Type);
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