using Recommendations.Abstractions.Entities;
using Recommendations.Abstractions.Repositories;
using Recommendations.Abstractions.Services.Interfaces;

namespace Recommendations.Abstractions.Services.Implementation;
public class UserService : IUserService
{
    private readonly IUserPortraitRepository _userPortraitRepository;

    public UserService(IUserPortraitRepository userPortraitRepository)
    {
        _userPortraitRepository = userPortraitRepository;
    }

    public async Task<UserPortrait> GetUserPortraitAsync(string userId)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(userId, nameof(userId));

        var userPortrait = await _userPortraitRepository.GetUserPortraitAsync(userId)
            ?? throw new NullReferenceException($"User with Id {userId} wasnnot found");

        return userPortrait;
    }

    public Task UpdateUserPortraitAsync(UserPortrait userPortrait, int pageSize = 10)
    {
        throw new NotImplementedException();
    }
}
