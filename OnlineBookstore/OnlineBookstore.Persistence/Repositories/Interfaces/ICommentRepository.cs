using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Persistence.Repositories.Interfaces;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IEnumerable<Comment>> GetCommentsByBookIdAsync(int bookId);

    bool IsUserWroteCommentForThisBook(string userId, int bookId);
}