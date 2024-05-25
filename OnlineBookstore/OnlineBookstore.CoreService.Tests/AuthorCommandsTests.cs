using OnlineBookstore.Application.Author;
using OnlineBookstore.Application.Author.Create;
using OnlineBookstore.Application.Author.Delete;
using OnlineBookstore.Application.Author.Update;
using OnlineBookstore.Application.Authors.Update;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Features.AuthorFeatures;

namespace OnlineBookstore.CoreService.Tests;

public class AuthorCommandsTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IAuthorCommandRepository> _authorCommandRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Faker _faker = new();
    private readonly IMapper _mapper;

    public AuthorCommandsTests()
    {
        _unitOfWorkMock.Setup(uow => uow.AuthorRepository)
            .Returns(_authorCommandRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync(CancellationToken.None))
            .Returns(Task.CompletedTask);
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateAuthorCommand, Author>().ReverseMap();
            cfg.CreateMap<UpdateAuthorCommand, Author>();
            cfg.CreateMap<GetAuthorDto, Author>().ReverseMap();
        });
        _mapper = config.CreateMapper();
    }
    
     [Fact]
    public async Task AddAuthorAsync_ShouldCallRepositoryCreateAndSaveMethods()
    {
        // Arrange
        var createAuthorDto = Builder<CreateAuthorCommand>.CreateNew()
            .With(a => a.Email = _faker.Random.Word() + "@email.com")
            .Build();
        
        var expectedAuthor = _mapper.Map<Author>(createAuthorDto);
        
        _mapperMock.Setup(m => m.Map<Author>(createAuthorDto))
            .Returns(expectedAuthor);
        _authorCommandRepoMock.Setup(ar => ar.AddAsync(expectedAuthor))
            .Returns(Task.CompletedTask);
        
        var handler = new CreateAuthorCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await handler.Handle(createAuthorDto, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
        _authorCommandRepoMock.Verify(uow => uow.AddAsync(expectedAuthor), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAuthorAsync_CallsUpdateAndSaveRepoMethods_IfExists()
    {
        // Arrange
        var updateAuthorDto = Builder<UpdateAuthorCommand>.CreateNew()
            .With(a => a.Email = _faker.Random.Word() + "@email.com")
            .Build();

        var expectedAuthor = _mapper.Map<Author>(updateAuthorDto);
        
        _mapperMock.Setup(m => m.Map<Author>(updateAuthorDto))
            .Returns(expectedAuthor);
        _authorCommandRepoMock.Setup(ar => ar.UpdateAsync(expectedAuthor))
            .Returns(Task.CompletedTask);

        var authorService = new UpdateAuthorCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await authorService.Handle(updateAuthorDto, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
        _authorCommandRepoMock.Verify(uow => 
            uow.UpdateAsync(expectedAuthor), Times.Once);
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
        _authorCommandRepoMock.Setup(ar => ar.DeleteAsync(author.Id))
            .Returns(Task.CompletedTask);

        var authorService = new DeleteAuthorCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await authorService.Handle(new DeleteAuthorCommand { AuthorId = existingAuthorId }, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
        _authorCommandRepoMock.Verify(uow => uow.DeleteAsync(author.Id), Times.Once);
    }
}