using OnlineBookstore.Application.Common;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Comments;

public interface ICommentQueryRepository : IGenericQueryRepository<Comment>
{
    Task<IEnumerable<Comment>> GetCommentsByBookIdAsync(int bookId);
}