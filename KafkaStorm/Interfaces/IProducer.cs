using System;
using System.Threading.Tasks;

namespace KafkaStorm.Services;

public interface IProducer : IDisposable
{
    Task ProduceAsync<T>(T message);
}