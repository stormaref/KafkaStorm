using System;
using KafkaStorm.Models;

namespace KafkaStorm.Interfaces;

public interface IMessageStore
{
    /// <summary>
    ///     Get last message that stored in memory
    /// </summary>
    /// <returns>Message object and id for corresponding method</returns>
    (bool Any, Guid Id, Message Message) GetLastMessage();

    /// <summary>
    ///     Remove message from store
    /// </summary>
    /// <param name="id">Id of message that you got from get message method</param>
    /// <returns>Succeeded or not</returns>
    bool RemoveMessage(Guid id);

    /// <summary>
    ///     Store message
    /// </summary>
    /// <param name="message">Message object</param>
    /// <typeparam name="TMessage">Message type</typeparam>
    /// <returns>Id corresponding to stored event</returns>
    Guid AddMessage<TMessage>(TMessage message, string? topicName = null);
}