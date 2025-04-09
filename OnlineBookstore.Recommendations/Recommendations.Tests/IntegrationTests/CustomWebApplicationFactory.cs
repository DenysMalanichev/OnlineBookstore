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
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"MongoDbSettings:ConnectionString", _mongoFixture.ConnectionString},
                {"MongoDbSettings:DatabaseName", "BookRecommendationTests"}
            });
        });

        builder.ConfigureServices(services =>
        {
            //services.AddSingleton(_mongoFixture.Client);
            //services.AddSingleton(_mongoFixture.Database);
            services.AddSingleton<MongoDbContext>(provider =>
                new MongoDbContext(_mongoFixture.ConnectionString, "BookRecommendationTests"));
            //services.AddSingleton(new Persistence.MongoDbContext(_mongoFixture.ConnectionString, "BookRecommendationTests"));

            //services.AddScoped<IUserPortraitRepository, UserPortraitRepository>(sp => 
            //    new UserPortraitRepository(new Persistence.MongoDbContext(_mongoFixture.ConnectionString, "BookRecommendationTests")));
        });
    }
}

