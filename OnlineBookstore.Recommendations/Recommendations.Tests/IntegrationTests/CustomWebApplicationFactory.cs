using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Recommendations.Persistence;

namespace Recommendations.Tests.IntegrationTests;
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private readonly MongoDbFixture _mongoFixture;

    public CustomWebApplicationFactory(MongoDbFixture mongoFixture)
    {
        _mongoFixture = mongoFixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"MongoDbSettings:ConnectionString", _mongoFixture.ConnectionString},
                {"MongoDbSettings:DatabaseName", "BookRecommendationTests"}
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddSingleton<MongoDbContext>(provider =>
                new MongoDbContext(_mongoFixture.ConnectionString, "BookRecommendationTests"));
        });
    }
}

