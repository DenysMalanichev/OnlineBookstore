using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace OnlineBookstore.Application.Configs;

[ExcludeFromCodeCoverage]
public static class ServicesConfiguration
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}