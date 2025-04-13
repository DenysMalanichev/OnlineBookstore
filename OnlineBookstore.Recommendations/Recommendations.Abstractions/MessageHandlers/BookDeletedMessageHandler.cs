using Recommendations.Abstractions.Messages;
using Recommendations.Abstractions.Repositories;

namespace Recommendations.Abstractions.MessageHandlers;
public class BookDeletedMessageHandler : IMessageHandler<BookDeletedMessage>
{
    private readonly IBookPortraitRepository _bookPortraitRepository;

    public BookDeletedMessageHandler(IBookPortraitRepository bookPortraitRepository)
    {
        _bookPortraitRepository = bookPortraitRepository;
    }

    public async Task HandleAsync(BookDeletedMessage message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        if (message.BookId <= 0)
        {
            throw new ArgumentNullException("Incorrect value of BookId. Should be non-null and greater then 0");
        }

        await _bookPortraitRepository.RemoveNormalizedBookAsync(message.BookId);
    }
}
