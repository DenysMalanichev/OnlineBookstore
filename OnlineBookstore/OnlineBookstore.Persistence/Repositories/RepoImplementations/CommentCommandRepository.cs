using OnlineBookstore.Application.Comments;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class CommentCommandRepository : GenericRepository<Comment>, ICommentCommandRepository
{
    private readonly DataContext _context;
    
    public CommentCommandRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public bool IsUserWroteCommentForThisBook(string userId, int bookId)
    {
        return _context.Comments.Any(c => c.BookId == bookId && c.UserId == userId);
    }
}