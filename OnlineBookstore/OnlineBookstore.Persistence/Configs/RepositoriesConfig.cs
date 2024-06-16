using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using OnlineBookstore.Application.Author;
using OnlineBookstore.Application.Books;
using OnlineBookstore.Application.Comments;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Genres;
using OnlineBookstore.Application.OrderDetails;
using OnlineBookstore.Application.Orders;
using OnlineBookstore.Application.Publishers;
using OnlineBookstore.Persistence.Repositories;
using OnlineBookstore.Persistence.Repositories.RepoImplementations;

namespace OnlineBookstore.Persistence.Configs;

[ExcludeFromCodeCoverage]
public static class RepositoriesConfig
{
    public static void AddUnitOfWork(this IServiceCollection service)
    {
        service.AddScoped<IAuthorCommandRepository, AuthorCommandRepository>();
        service.AddScoped<IAuthorQueryRepository, AuthorQueryRepository>();
        
        service.AddScoped<ICommentCommandRepository, CommentCommandRepository>();
        service.AddScoped<ICommentQueryRepository, CommentQueryRepository>();
        
        service.AddScoped<IBookCommandRepository, BookCommandRepository>();
        service.AddScoped<IBookQueryRepository, BookQueryRepository>();
        
        service.AddScoped<IGenreCommandRepository, GenreCommandRepository>();
        service.AddScoped<IGenreQueryRepository, GenreQueryRepository>();

        service.AddScoped<IOrderCommandRepository, OrderCommandRepository>();
        service.AddScoped<IOrderQueryRepository, OrderQueryRepository>();
        
        service.AddScoped<IOrderDetailCommandRepository, OrderDetailCommandRepository>();
        service.AddScoped<IOrderDetailQueryRepository, OrderDetailQueryRepository>();
        
        service.AddScoped<IPublisherCommandRepository, PublisherCommandRepository>();
        service.AddScoped<IPublisherQueryRepository, PublisherQueryRepository>();
        
        service.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}