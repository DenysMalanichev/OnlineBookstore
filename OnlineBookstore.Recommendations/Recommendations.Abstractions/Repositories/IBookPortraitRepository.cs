using Recommendations.Abstractions.Entities;

namespace Recommendations.Abstractions.Repositories;
public interface IBookPortraitRepository
{
    public Task<IList<int>> GetRecommendedBooksIdsAsync(UserPortrait userPortrait, int pageNumber = 1, int pageSize = 10);
    public Task UpsertNormalizedBooksAsync(BookPortrait bookPortrait);
    public Task UpdateNormalizedBookPurchaseNumberAsync(int bookId);
    public Task RemoveNormalizedBookAsync(int bookId);
}
