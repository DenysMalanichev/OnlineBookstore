using Recommendations.Abstractions.Messages;

namespace Recommendations.Abstractions.MessageHandlers;
public class BookDeletedMessageHandler : IMessageHandler<BookDeletedMessage>
{
    public Task HandleAsync(BookDeletedMessage message)
    {
        throw new NotImplementedException();
    }
}
