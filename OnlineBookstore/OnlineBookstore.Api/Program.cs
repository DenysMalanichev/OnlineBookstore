using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Application.Configs;
using OnlineBookstore.Application.Services.Implementation;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Configs;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Extentions;
using OnlineBookstore.Features.Mapper;
using OnlineBookstore.Features.UserFeatures.Options;
using OnlineBookstore.Middleware;
using OnlineBookstore.Persistence.Configs;
using OnlineBookstore.Persistence.Context;

public class Program
{
    public static void Main(string[] args)
    {
        const string allowFrontEndSpecificOrigins = "_frontEndSpecificOrigins";

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

        builder.AddCorsPolicy(allowFrontEndSpecificOrigins);

        builder.Services.AddAuthConfigurations(builder.Configuration);

        builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

        builder.Services.AddUnitOfWork();
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));
        builder.Services.AddCustomServices();

        builder.Services.AddControllers();

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        var connectionString = builder.Configuration.GetConnectionString("ConnStr");
        builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(connectionString)
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information));


        builder.Services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        RolesDbInitializer.SeedRolesToDbAsync(app).Wait();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors(allowFrontEndSpecificOrigins);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}