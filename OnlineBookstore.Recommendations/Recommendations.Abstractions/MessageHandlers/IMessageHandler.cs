namespace Recommendations.Abstractions.MessageHandlers;
// <summary>
/// Interface for message handlers that process messages from Kafka
/// </summary>
/// <typeparam name="T">The type of message to handle</typeparam>
public interface IMessageHandler<in T>
{
    /// <summary>
    /// Handles a message asynchronously
    /// </summary>
    /// <param name="message">The message to handle</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task HandleAsync(T message);
}
