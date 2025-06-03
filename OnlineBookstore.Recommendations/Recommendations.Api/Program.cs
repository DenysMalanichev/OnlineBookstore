using Recommendations.Abstractions.MessageHandlers;
using Recommendations.Abstractions.Messages;
using Recommendations.Abstractions.Repositories;
using Recommendations.Abstractions.Services.Implementation;
using Recommendations.Abstractions.Services.Interfaces;
using Recommendations.Api;
using Recommendations.Api.Middleware;
using Recommendations.Api.Settings;
using Recommendations.Persistence;
using Recommendations.Persistence.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Retrieve MongoDB connection settings
var connectionString = builder.Configuration.GetConnectionString("MongoDbRecommendations")
    ?? throw new ArgumentNullException("No configuration for MongoDbRecommendations connection string");
var databaseName = builder.Configuration["DbNames:RecommendationsDbName"]
    ?? throw new ArgumentNullException("No configuration for MongoDbRecommendations DB name");

// Register MongoDbContext as a singleton
builder.Services.AddSingleton<MongoDbContext>(provider => new MongoDbContext(connectionString, databaseName));

// Add response caching
builder.Services.AddResponseCaching();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookPortraitRepository, BookPortraitRepository>();
builder.Services.AddScoped<IUserPortraitRepository, UserPortraitRepository>();
// Add Kafka settings
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings"));

// Register message handlers
builder.Services.AddScoped<IMessageHandler<BookDeletedMessage>, BookDeletedMessageHandler>();
builder.Services.AddScoped<IMessageHandler<BookUpsertedMessage>, BookUpsertedMessageHandler>();
builder.Services.AddScoped<IMessageHandler<BookPurchasedMessage>, BookPurchasedMessageHandler>();
builder.Services.AddScoped<IMessageHandler<UserUpsertMessage>, UserUpsertMessageHandler>();

// Register Kafka consumer service
builder.Services.AddHostedService<KafkaConsumerService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseResponseCaching();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { } // For integration tests to work
