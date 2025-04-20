using Recommendations.Abstractions.Entities;

namespace Recommendations.Abstractions.Repositories;
public interface IUserPortraitRepository
{
    public Task<UserPortrait> GetUserPortraitAsync(string id);
    public Task UpsertUserPortraitDataAsync(UserPortrait userPortrait);
}
