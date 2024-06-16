using System.Diagnostics.CodeAnalysis;

namespace OnlineBookstore.Configs;

[ExcludeFromCodeCoverage]
public static class CorsConfiguration
{
    public static void AddCorsPolicy(this WebApplicationBuilder builder, string name)
    {
        var origin = builder.Configuration.GetSection("Cors")["FrontEndUrl"]
                     ?? throw new NullReferenceException("Origin is not configured.");
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: name,
                policy  =>
                {
                    policy.WithOrigins(
                            origin)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }
}