using Recommendations.Abstractions.Entities;
using Recommendations.Abstractions.Repositories;

namespace Recommendations.Persistence.Repositories;
public class BookPortraitRepository : IBookPortraitRepository
{
    public Task<IList<int>> GetRecommendedBooksIdsAsync(UserPortrait userPortrait)
    {
        throw new NotImplementedException();
    }

    public Task<IList<int>> UpdateNormalizedBooksAsync(BookPortrait userPortrait)
    {
        throw new NotImplementedException();
    }
}
