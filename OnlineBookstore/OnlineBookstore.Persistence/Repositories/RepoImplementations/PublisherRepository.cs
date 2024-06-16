using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;
using OnlineBookstore.Persistence.Repositories.Interfaces;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class PublisherRepository : GenericRepository<Publisher>, IPublisherRepository
{
    public PublisherRepository(DataContext context)
        : base(context)
    {
    }
}