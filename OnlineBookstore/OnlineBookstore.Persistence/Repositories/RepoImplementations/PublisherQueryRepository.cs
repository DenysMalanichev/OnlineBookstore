using OnlineBookstore.Application.Publishers;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class PublisherQueryRepository : GenericQueryRepository<Publisher>, IPublisherQueryRepository
{
    public PublisherQueryRepository(DataContext context) : base(context)
    {
    }
}