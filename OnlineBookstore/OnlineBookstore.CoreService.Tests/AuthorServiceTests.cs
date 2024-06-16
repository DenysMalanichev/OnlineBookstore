using OnlineBookstore.Features.AuthorFeatures;

namespace OnlineBookstore.CoreService.Tests;

public class AuthorServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IAuthorRepository> _authorRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Faker _faker = new();
    private readonly IMapper _mapper;

    public AuthorServiceTests()
    {
        _unitOfWorkMock.Setup(uow => uow.AuthorRepository)
            .Returns(_authorRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync())
            .Returns(Task.CompletedTask);
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateAuthorDto, Author>().ReverseMap();
            cfg.CreateMap<UpdateAuthorDto, Author>();
            cfg.CreateMap<GetAuthorDto, Author>().ReverseMap();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task AddAuthorAsync_ShouldCallRepositoryCreateAndSaveMethods()
    {
        // Arrange
        var createAuthorDto = Builder<CreateAuthorDto>.CreateNew()
            .With(a => a.Email = _faker.Random.Word() + "@email.com")
            .Build();
        
        var expectedAuthor = _mapper.Map<Author>(createAuthorDto);
        
        _mapperMock.Setup(m => m.Map<Author>(createAuthorDto))
            .Returns(expectedAuthor);
        _authorRepoMock.Setup(ar => ar.AddAsync(expectedAuthor))
            .Returns(Task.CompletedTask);

        var authorService = new AuthorService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await authorService.AddAuthorAsync(createAuthorDto);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _authorRepoMock.Verify(uow => uow.AddAsync(expectedAuthor), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthorAsync_ThrowsEntityNotFoundException_IfDoesntExist()
    {
        // Arrange
        var updateAuthorDto = Builder<UpdateAuthorDto>.CreateNew()
            .With(a => a.Email = _faker.Random.Word() + "@email.com")
            .Build();
        
        var authorService = new AuthorService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await authorService.UpdateAuthorAsync(updateAuthorDto));
    }
    
    [Fact]
    public async Task UpdateAuthorAsync_CallsUpdateAndSaveRepoMethods_IfExists()
    {
        // Arrange
        var updateAuthorDto = Builder<UpdateAuthorDto>.CreateNew()
            .With(a => a.Email = _faker.Random.Word() + "@email.com")
            .Build();

        var expectedAuthor = _mapper.Map<Author>(updateAuthorDto);
        
        _mapperMock.Setup(m => m.Map<Author>(updateAuthorDto))
            .Returns(expectedAuthor);
        _authorRepoMock.Setup(ar => ar.UpdateAsync(expectedAuthor))
            .Returns(Task.CompletedTask);
        _authorRepoMock.Setup(ar => ar.GetByIdAsync(updateAuthorDto.Id, true))!
            .ReturnsAsync(expectedAuthor);

        var authorService = new AuthorService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await authorService.UpdateAuthorAsync(updateAuthorDto);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _authorRepoMock.Verify(uow => uow.UpdateAsync(expectedAuthor), Times.Once);
    }
        
        
    [Fact]
    public async Task GetAuthorByIdAsync_ThrowsEntityNotFoundException_IfDoesntExist()
    {
        // Arrange
        const int notExistingAuthorId = -100; 
        
        var authorService = new AuthorService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await authorService.GetAuthorByIdAsync(notExistingAuthorId));
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

        var authorService = new AuthorService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        var returnedAuthorDto = await authorService.GetAuthorByIdAsync(existingAuthorId);
        
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

        var authorService = new AuthorService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        var returnedAuthorDto = await authorService.GetAllAuthorsAsync();
        
        // Assert
        returnedAuthorDto.Should().BeEquivalentTo(expectedAuthorDto);
    }
    
    [Fact]
    public async Task DeleteAuthorAsync_ThrowsEntityNotFoundException_IfDoesntExist()
    {
        // Arrange
        const int notExistingAuthorId = -100; 
        
        var authorService = new AuthorService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await authorService.DeleteAuthorAsync(notExistingAuthorId));
    }
    
    [Fact]
    public async Task DeleteAuthorAsync_ReturnsGetAuthorDto_IfExist()
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
        _authorRepoMock.Setup(ar => ar.DeleteAsync(author))
            .Returns(Task.CompletedTask);

        var authorService = new AuthorService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await authorService.DeleteAuthorAsync(existingAuthorId);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _authorRepoMock.Verify(uow => uow.DeleteAsync(author), Times.Once);
    }
}