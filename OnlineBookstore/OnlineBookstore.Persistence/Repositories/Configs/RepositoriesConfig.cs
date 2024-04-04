using Microsoft.Extensions.DependencyInjection;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.Interfaces;
using OnlineBookstore.Persistence.Repositories.RepoImplementations;

namespace OnlineBookstore.Persistence.Repositories.Configs;

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