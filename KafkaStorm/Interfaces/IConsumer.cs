using System.Threading;
using System.Threading.Tasks;

namespace KafkaStorm.Interfaces;

public interface IConsumer<in T> where T : class
{
    Task Handle(T message, CancellationToken cancellationToken);
}