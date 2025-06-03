using Recommendations.Abstractions.Entities;

namespace Recommendations.Abstractions.Services.Interfaces;
public interface IUserService
{
    public Task UpdateUserPortraitAsync(UserPortrait userPortrait, int pageSize = 10);

    public Task<UserPortrait> GetUserPortraitAsync(string userId);
}
