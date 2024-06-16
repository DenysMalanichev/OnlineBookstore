using OnlineBookstore.Application.Author;
using OnlineBookstore.Application.Author.GetAll;
using OnlineBookstore.Application.Author.GetAuthorById;
using OnlineBookstore.Application.Authors.GetAuthorById;
using OnlineBookstore.Features.AuthorFeatures;

namespace OnlineBookstore.CoreService.Tests;

public class AuthorQueriesTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IAuthorQueryRepository> _authorRepoMock = new();
    private readonly Faker _faker = new();
    private readonly IMapper _mapper;

    public AuthorQueriesTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GetAuthorDto, Author>().ReverseMap();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task GetAuthorByIdAsync_ThrowsEntityNotFoundException_IfDoesntExist()
    {
        // Arrange
        const int notExistingAuthorId = -100; 
        
        var authorService = new GetAuthorByIdQueryHandler(_authorRepoMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await authorService.Handle(
                new GetAuthorByIdQuery { AuthorId = notExistingAuthorId }, 
                CancellationToken.None));
    }
    
    [Fact]
    public async Task GetAuthorByIdAsync_ReturnsGetAuthorDto_IfExist()
    {
        // Arrange
        const int existingAuthorId = 100; 
        var author = Builder<Author>.CreateNew()
            .With(a => a.Email = _faker.Random.Word() + "@email.com")
            .With(a => a.Id = existingAuthorId)
            .Build();

        var expectedAuthorDto = _mapper.Map<GetAuthorDto>(author);
        
        _mapperMock.Setup(m => m.Map<GetAuthorDto>(author))
            .Returns(expectedAuthorDto);
        _authorRepoMock.Setup(ar => ar.GetByIdAsync(existingAuthorId, false))!
            .ReturnsAsync(author);

        var authorService = new GetAuthorByIdQueryHandler(_authorRepoMock.Object, _mapperMock.Object);
        
        // Act
        var returnedAuthorDto = await authorService.Handle(
            new GetAuthorByIdQuery { AuthorId = existingAuthorId }, 
            CancellationToken.None);
        
        // Assert
        returnedAuthorDto.Should().BeEquivalentTo(expectedAuthorDto);
    }
    
    [Fact]
    public async Task GetAllAuthorsAsync_ReturnsIEnumerableOfGetAuthorDto()
    {
        // Arrange
        var authors = Builder<Author>.CreateListOfSize(10)
            .All()
            .With(a => a.Email = _faker.Random.Word() + "@email.com")
            .Build();

        var expectedAuthorDto = _mapper.Map<IEnumerable<GetAuthorDto>>(authors).ToList();
        
        _mapperMock.Setup(m => m.Map<IEnumerable<GetAuthorDto>>(authors))
            .Returns(expectedAuthorDto);
        _authorRepoMock.Setup(ar => ar.GetAllAsync())
            .ReturnsAsync(authors);

        var authorService = new GetAllAuthorsQueryHandler(_authorRepoMock.Object, _mapperMock.Object);
        
        // Act
        var returnedAuthorDto = await authorService
            .Handle(new GetAllAuthorsQuery(), CancellationToken.None);
        
        // Assert
        returnedAuthorDto.Should().BeEquivalentTo(expectedAuthorDto);
    }
}