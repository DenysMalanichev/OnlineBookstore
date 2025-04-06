using Recommendations.Abstractions.Entities;

namespace Recommendations.Abstractions.Repositories;
public interface IBookPortraitRepository
{
    public Task<IList<int>> GetRecommendedBooksIdsAsync(UserPortrait userPortrait, int pageSize = 10, int pageNumber = 1);
    public Task<IList<int>> UpdateNormalizedBooksAsync(BookPortrait bookPortrait);
}
