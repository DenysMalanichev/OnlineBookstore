using Recommendations.Abstractions.Messages;
using Recommendations.Abstractions.Repositories;

namespace Recommendations.Abstractions.MessageHandlers;
public class BookPurchasedMessageHandler : IMessageHandler<BookPurchasedMessage>
{
    private readonly IBookPortraitRepository _bookPortraitRepository;
    private readonly IUserPortraitRepository _userPortraitRepository;

    public BookPurchasedMessageHandler(
        IBookPortraitRepository bookPortraitRepository,
        IUserPortraitRepository userPortraitRepository)
    {
        _bookPortraitRepository = bookPortraitRepository;
        _userPortraitRepository = userPortraitRepository;
    }

    public async Task HandleAsync(BookPurchasedMessage message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        if(message.BookId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(message.BookId));
        }

        await _bookPortraitRepository.UpdateNormalizedBookPurchaseNumberAsync(message.BookId);

        var user = await _userPortraitRepository.GetUserPortraitAsync(message.UserId);
        user.PurchasedBooks.Add(message.BookId);
        await _userPortraitRepository.UpsertUserPortraitDataAsync(user);
    }
}
