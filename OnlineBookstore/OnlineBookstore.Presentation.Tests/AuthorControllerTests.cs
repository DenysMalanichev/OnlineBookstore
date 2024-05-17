using System.Data.Entity;
using System.Text;
using FizzWare.NBuilder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using OnlineBookstore.Features.AuthorFeatures;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Presentation.Tests;

public class AuthorControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AuthorControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateAuthorAsync_CreateNewAuthorInDb()
    {
        // Arrange
        var createNewAuthorDto = Builder<CreateAuthorDto>.CreateNew().Build();

        var content = new StringContent(
            JsonConvert.SerializeObject(createNewAuthorDto), Encoding.UTF8, "application/json");
        const string url = "/api/authors";

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        Assert.NotNull(await dbContext.Authors.FirstOrDefaultAsync(a =>
            a.FirstName == createNewAuthorDto.FirstName && a.LastName == createNewAuthorDto.LastName));
    }
}