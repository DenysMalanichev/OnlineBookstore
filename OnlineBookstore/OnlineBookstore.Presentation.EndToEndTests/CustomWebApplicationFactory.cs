using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Presentation.EndToEndTests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    public CustomWebApplicationFactory()
    {
        ClientOptions.BaseAddress = new Uri("http://localhost/");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<DataContext>(_ =>
            {
                var options = new DbContextOptionsBuilder<DataContext>()
                    .UseInMemoryDatabase(_databaseName)
                    .Options;
                var db = new DataContext(options);
                db.Database.EnsureDeleted(); // Ensure the database is clean
                db.Database.EnsureCreated(); // Recreate the database
                return db;
            });
        });
    }
}