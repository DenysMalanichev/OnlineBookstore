using Microsoft.Extensions.DependencyInjection;
using OnlineBookstore.Persistence.Repositories;
using OnlineBookstore.Persistence.Repositories.Interfaces;
using OnlineBookstore.Persistence.Repositories.RepoImplementations;

namespace OnlineBookstore.Persistence.Configs;

public static class RepositoriesConfig
{
    public static void AddUnitOfWork(this IServiceCollection service)
    {
        service.AddScoped<IAuthorRepository, AuthorRepository>();
        service.AddScoped<IBookRepository, BookRepository>();
        service.AddScoped<ICommentRepository, CommentRepository>();
        service.AddScoped<IGenreRepository, GenreRepository>();
        service.AddScoped<IOrderRepository, OrderRepository>();
        service.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
        service.AddScoped<IPublisherRepository, PublisherRepository>();
        
        service.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}