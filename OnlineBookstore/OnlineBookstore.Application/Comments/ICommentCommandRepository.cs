using OnlineBookstore.Application.Common;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Comments;

public interface ICommentCommandRepository : IGenericRepository<Comment>
{
    bool IsUserWroteCommentForThisBook(string userId, int bookId);
}