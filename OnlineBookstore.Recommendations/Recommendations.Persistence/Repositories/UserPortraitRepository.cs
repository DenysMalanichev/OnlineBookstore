using MongoDB.Driver;
using Recommendations.Abstractions.Entities;
using Recommendations.Abstractions.Repositories;

namespace Recommendations.Persistence.Repositories;
public class UserPortraitRepository : IUserPortraitRepository
{
    private readonly IMongoCollection<UserPortrait> _userPortraits;

    public UserPortraitRepository(MongoDbContext mongoDbContext)
    {
        _userPortraits = mongoDbContext.UserPortraits;
    }

    public async Task<UserPortrait> GetUserPortraitAsync(string userId)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(userId, nameof(userId));

        var filter = Builders<UserPortrait>.Filter.Eq(u => u.UserId, userId);

        var userPortrait = await _userPortraits.Find(filter).FirstOrDefaultAsync();

        if (userPortrait == null)
        {
            throw new NullReferenceException($"User with Id {userId} was not found");
        }

        return userPortrait;
    }

    public Task UpdateUserPortraitDataAsync(UserPortrait userPortrait)
    {
        throw new NotImplementedException();
    }
}
