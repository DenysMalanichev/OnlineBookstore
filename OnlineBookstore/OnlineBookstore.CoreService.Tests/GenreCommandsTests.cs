using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Genres;
using OnlineBookstore.Application.Genres.Create;
using OnlineBookstore.Application.Genres.Delete;
using OnlineBookstore.Application.Genres.Dtos;
using OnlineBookstore.Application.Genres.Update;

namespace OnlineBookstore.CoreService.Tests;

public class GenreCommandsTests
{
     private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IGenreCommandRepository> _genreRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;

    public GenreCommandsTests()
    {
        _unitOfWorkMock.Setup(uow => uow.GenreRepository)
            .Returns(_genreRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync(CancellationToken.None))
            .Returns(Task.CompletedTask);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateGenreCommand, Genre>().ReverseMap();
            cfg.CreateMap<UpdateGenreCommand, Genre>().ReverseMap();
        });
            
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task AddGenreAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        var createGenreDto = Builder<CreateGenreCommand>.CreateNew().Build();
        var genre = _mapper.Map<Genre>(createGenreDto);

        _mapperMock.Setup(m => m.Map<Genre>(createGenreDto))
            .Returns(genre);

        var genreService = new CreateGenreCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await genreService.Handle(createGenreDto, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
        _genreRepoMock.Verify(gr => gr.AddAsync(genre), Times.Once);
    }
    
    [Fact]
    public async Task UpdateGenreAsync_ShouldCallUpdateAndSaveRepoMethods()
    {
        // Arrange
        var updateGenreDto = Builder<UpdateGenreCommand>.CreateNew().Build();
        var genre = _mapper.Map<Genre>(updateGenreDto);

        _mapperMock.Setup(m => m.Map<Genre>(updateGenreDto))
            .Returns(genre);

        var genreService = new UpdateGenreCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await genreService.Handle(updateGenreDto, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
        _genreRepoMock.Verify(gr => gr.UpdateAsync(genre), Times.Once);
    }
    
    [Fact]
    public async Task DeleteGenreAsync_ShouldReturnCallDeleteAndSaveRepoMethods()
    {
        // Arrange
        const int genreId = 10;
        var genre = Builder<Genre>.CreateNew()
            .With(g => g.Id = genreId)
            .Build();

        var genreService = new DeleteGenreCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await genreService.Handle(new DeleteGenreCommand { GenreId = genreId}, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
        _genreRepoMock.Verify(gr => gr.DeleteAsync(genre.Id), Times.Once);
    }
}