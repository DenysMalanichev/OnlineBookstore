using Recommendations.Abstractions.Entities;
using Recommendations.Abstractions.Messages;
using Recommendations.Abstractions.Repositories;

namespace Recommendations.Abstractions.MessageHandlers;
public class UserUpsertMessageHandler : IMessageHandler<UserUpsertMessage>
{
    private readonly IUserPortraitRepository _userPortraitRepository;

    public UserUpsertMessageHandler(IUserPortraitRepository userPortraitRepository)
    {
        _userPortraitRepository = userPortraitRepository;
    }

    public async Task HandleAsync(UserUpsertMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var userPortrait = new UserPortrait
        {
            UserId = message.UserId,
            PurchasedBooks = message.PurchasedBooks,
            PreferedAuthoreIds = message.PreferedAuthoreIds,
            PreferedGenreIds = message.PreferedGenreIds,
            PreferedLanguages = message.PreferedLanguages,
            IsPaperbackPrefered = message.IsPaperbackPrefered,
        };

        await _userPortraitRepository.UpsertUserPortraitDataAsync(userPortrait);
    }
}
