using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Application.Comments;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class CommentQueryRepository : GenericQueryRepository<Comment>, ICommentQueryRepository
{
    private readonly DataContext _context;
    
    public CommentQueryRepository(DataContext context) : base(context)
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
}