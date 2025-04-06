using Recommendations.Abstractions.Entities;
using Recommendations.Abstractions.Repositories;
using Recommendations.Abstractions.Services.Interfaces;

namespace Recommendations.Abstractions.Services.Implementation;
public class BookService : IBookService
{
    private IUserService _userService;
    private IBookPortraitRepository _bookRepository;

    public BookService(IUserService userService, IBookPortraitRepository bookPortraitRepository)
    {
        _userService = userService;
        _bookRepository = bookPortraitRepository;
    }

    public async Task<IList<int>> GenerateRecommendationsAsync(string userId, int pageSize = 10)
    {
        var userPortrait = await _userService.GetUserPortraitAsync(userId)
            ?? throw new NullReferenceException($"User with Id {userId} wasnnot found");

        return await _bookRepository.GetRecommendedBooksIdsAsync(userPortrait, pageSize);
    }

    public Task UpdateNormilizedBooksDataAsync(BookPortrait bookPortrait)
    {
        throw new NotImplementedException();
    }
}
