using OnlineBookstore.Application.Publishers;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class PublisherCommandRepository : GenericRepository<Publisher>, IPublisherCommandRepository
{
    public PublisherCommandRepository(DataContext context) : base(context)
    {
    }
}