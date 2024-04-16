using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.Interfaces;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    private readonly DataContext _context;
    
    public CommentRepository(DataContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Comment>> GetCommentsByBookIdAsync(int bookId)
    {
        return await _context.Comments
            .AsNoTracking()
            .Include(c => c.Book)
            .Where(c => c.Book.Id == bookId)
            .ToListAsync();
    }

    public bool IsUserWroteCommentForThisBook(string userId, int bookId)
    {
        return _context.Comments.Any(c => c.BookId == bookId && c.UserId == userId);
    }
}