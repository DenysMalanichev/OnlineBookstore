using OnlineBookstore.Features.OrderFeatures;
using OnlineBookstore.Persistence.Repositories.RepoImplementations;

namespace OnlineBookstore.Application.Services.Interfaces;

public interface IOrderService
{
    Task<GetOrderDto> GetUsersActiveOrderAsync(string userId);

    Task<IEnumerable<GetOrderDto>> GetUsersOrdersAsync(string userId);

    Task<IEnumerable<BooksOrdersStatisticsDto>> GetBooksOrderStatisticsAsync(int bookId);

    Task CloseUsersOrderAsync(CreateOrderDto createOrderDto, string userId);
}