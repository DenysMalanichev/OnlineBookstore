using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.Interfaces;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    public CommentRepository(DataContext context)
        : base(context)
    {
    }
}