using Microsoft.Extensions.DependencyInjection;
using OnlineBookstore.Application.Services.Implementation;
using OnlineBookstore.Application.Services.Interfaces;

namespace OnlineBookstore.Application.Configs;

public static class ServicesConfiguration
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IPublisherService, PublisherService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderDetailService, OrderDetailService>();
        services.AddScoped<IUserService, UserService>();
    }
}