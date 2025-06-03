using Recommendations.Abstractions.Entities;
using Recommendations.Abstractions.Messages;
using Recommendations.Abstractions.Repositories;

namespace Recommendations.Abstractions.MessageHandlers;
public class BookUpsertedMessageHandler : IMessageHandler<BookUpsertedMessage>
{
    private readonly IBookPortraitRepository _bookPortraitRepository;

    public BookUpsertedMessageHandler(IBookPortraitRepository bookPortraitRepository)
    {
        _bookPortraitRepository = bookPortraitRepository;
    }

    public async Task HandleAsync(BookUpsertedMessage message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        var bookPortraite = new BookPortrait
        {
            BookId = message.BookId,
            Language = message.Language,
            GenreIds = message.GenreIds,
            AuthorId = message.AuthorId,
            Rating = message.Rating,
            PurchaseNumber = message.PurchaseNumber,
            IsPaperback = message.IsPaperback,
        };
        await _bookPortraitRepository.UpsertNormalizedBooksAsync(bookPortraite);
    }
}
