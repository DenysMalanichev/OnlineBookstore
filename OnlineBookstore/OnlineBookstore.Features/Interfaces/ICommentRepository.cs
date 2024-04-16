using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Features.Interfaces;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IEnumerable<Comment>> GetCommentsByBookIdAsync(int bookId);

    bool IsUserWroteCommentForThisBook(string userId, int bookId);
}