using System;

namespace KafkaStorm.Interfaces;

public interface IMessageStore
{
    /// <summary>
    /// Get last message that stored in memory
    /// </summary>
    /// <returns>Message object and id for corresponding method</returns>
    (Guid Id, object Message) GetLastMessage();

    /// <summary>
    /// Remove message from store
    /// </summary>
    /// <param name="id">Id of message that you got from get message method</param>
    /// <returns>Succeeded or not</returns>
    bool RemoveMessage(Guid id);

    /// <summary>
    /// Add message to message store
    /// </summary>
    /// <param name="message">Message object</param>
    /// <returns>Id of stored message</returns>
    Guid AddMessage(object message);
}