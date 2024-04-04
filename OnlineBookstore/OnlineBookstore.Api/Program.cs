using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Application.Configs;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.Mapper;
using OnlineBookstore.Middleware;
using OnlineBookstore.Persistence.Context;
using OnlineBookstore.Persistence.Repositories.Configs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddUnitOfWork();
builder.Services.AddCustomServices();

builder.Services.AddControllers();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

var connectionString = builder.Configuration.GetConnectionString("ConnStr");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();