using Recommendations.Abstractions.Entities;

namespace Recommendations.Abstractions.Services.Interfaces;
public interface IBookService
{
    public Task<IList<int>> GenerateRecommendationsAsync(string userId, int pageSize = 10);
    public Task UpdateNormilizedBooksDataAsync(BookPortrait bookPortrait);
}
