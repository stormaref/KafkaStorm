using System;
using System.Threading.Tasks;
using KafkaStorm.Models;

namespace KafkaStorm.Interfaces;

public interface IProducer : IDisposable
{
    /// <summary>
    ///     Produce message to kafka
    /// </summary>
    /// <param name="message">Message object</param>
    /// <param name="topicName">Name of topic</param>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <returns></returns>
    Task Produce<TMessage>(TMessage message, string? topicName = null);

    /// <summary>
    ///     Produce a message in kafka without queuing (may produce exception)
    /// </summary>
    /// <param name="message">Message object</param>
    /// <param name="topicName">Topic that you want to send the message in</param>
    /// <typeparam name="TMessage">Message type</typeparam>
    /// <returns></returns>
    Task ProduceNowAsync<TMessage>(TMessage message, string? topicName = null);

    internal Task ProduceNowAsync(StoredMessage message);
}