using System;
using System.Threading.Tasks;

namespace KafkaStorm.Interfaces;

public interface IProducer : IDisposable
{
    /// <summary>
    /// Produce message to kafka
    /// </summary>
    /// <param name="message">Message object</param>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <returns></returns>
    Task Produce<TMessage>(TMessage message);

    Task ProduceNow<TMessage>(TMessage message);
}