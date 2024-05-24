using OnlineBookstore.Application.Books;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Genres.Dtos;
using OnlineBookstore.Features.GenreFeatures;

namespace OnlineBookstore.CoreService.Tests;

public class GenreServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IGenreRepository> _genreRepoMock = new();
    private readonly Mock<IBookCommandRepository> _bookRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;

    public GenreServiceTests()
    {
        _unitOfWorkMock.Setup(uow => uow.GenreRepository)
            .Returns(_genreRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.BookRepository)
            .Returns(_bookRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync())
            .Returns(Task.CompletedTask);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateGenreDto, Genre>().ReverseMap();
            cfg.CreateMap<GetGenreDto, Genre>().ReverseMap();
            cfg.CreateMap<GetBriefGenreDto, Genre>().ReverseMap();
            cfg.CreateMap<UpdateGenreDto, Genre>().ReverseMap();
        });
            
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task AddGenreAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        var createGenreDto = Builder<CreateGenreDto>.CreateNew().Build();
        var genre = _mapper.Map<Genre>(createGenreDto);

        _mapperMock.Setup(m => m.Map<Genre>(createGenreDto))
            .Returns(genre);

        var genreService = new GenreService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await genreService.AddGenreAsync(createGenreDto);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _genreRepoMock.Verify(gr => gr.AddAsync(genre), Times.Once);
    }
    
    [Fact]
    public async Task UpdateGenreAsync_ShouldCallUpdateAndSaveRepoMethods()
    {
        // Arrange
        var updateGenreDto = Builder<UpdateGenreDto>.CreateNew().Build();
        var genre = _mapper.Map<Genre>(updateGenreDto);

        _mapperMock.Setup(m => m.Map<Genre>(updateGenreDto))
            .Returns(genre);
        _genreRepoMock.Setup(gr => gr.GetByIdAsync(updateGenreDto.Id, true))!
            .ReturnsAsync(genre);

        var genreService = new GenreService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await genreService.UpdateGenreAsync(updateGenreDto);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _genreRepoMock.Verify(gr => gr.UpdateAsync(genre), Times.Once);
    }
    
    [Fact]
    public async Task UpdateGenreAsync_ThrowsEntityNotFoundException_IfGenreDoesntExist()
    {
        // Arrange
        const int notExistingGenreId = -100;
        var updateGenreDto = Builder<UpdateGenreDto>.CreateNew()
            .With(g => g.Id = notExistingGenreId)
            .Build();

        _genreRepoMock.Setup(gr => gr.GetByIdAsync(notExistingGenreId, true))!
            .ReturnsAsync((Genre)null!);

        var genreService = new GenreService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await genreService.UpdateGenreAsync(updateGenreDto));
    }
    
    [Fact]
    public async Task GetGenreByIdAsync_ThrowsEntityNotFoundException_IfGenreDoesntExist()
    {
        // Arrange
        const int notExistingGenreId = -100;

        _genreRepoMock.Setup(gr => gr.GetByIdAsync(notExistingGenreId, true))!
            .ReturnsAsync((Genre)null!);

        var genreService = new GenreService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await genreService.GetGenreByIdAsync(notExistingGenreId));
    }
    
    [Fact]
    public async Task GetGenreByIdAsync_ShouldReturnCorrespondingGenreDto()
    {
        // Arrange
        const int genreId = 100;
        var genre = Builder<Genre>.CreateNew().Build();
        var genreDto = _mapper.Map<GetGenreDto>(genre);

        _mapperMock.Setup(m => m.Map<GetGenreDto>(genre))
            .Returns(genreDto);
        _genreRepoMock.Setup(gr => gr.GetByIdAsync(genreId, false))!
            .ReturnsAsync(genre);

        var genreService = new GenreService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        var returnedGenreDto = await genreService.GetGenreByIdAsync(genreId);
        
        // Assert
        returnedGenreDto.Should().BeEquivalentTo(genreDto);
    }
    
    [Fact]
    public async Task GetAllGenresAsync_ShouldReturnIEnumerableOfGenres()
    {
        // Arrange
        var genres = Builder<Genre>.CreateListOfSize(10).Build();
        var genresDto = _mapper.Map<IEnumerable<GetGenreDto>>(genres);

        var getGenresDto = genresDto as GetGenreDto[] ?? genresDto.ToArray();
        _mapperMock.Setup(m => m.Map<IEnumerable<GetGenreDto>>(genres))
            .Returns(getGenresDto);
        _genreRepoMock.Setup(gr => gr.GetAllAsync())
            .ReturnsAsync(genres);

        var genreService = new GenreService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        var returnedGenresDto = await genreService.GetAllGenresAsync();
        
        // Assert
        returnedGenresDto.Should().BeEquivalentTo(getGenresDto);
    }
    
    [Fact]
    public async Task GetGenresByBookAsync_ShouldReturnIEnumerableOfGenresByBook()
    {
        // Arrange
        const int bookId = 10;
        var genres = Builder<Genre>.CreateListOfSize(10).Build();
        var book = Builder<Book>.CreateNew()
            .With(b => b.Genres = genres)
            .Build();
        var genresDto = _mapper.Map<IEnumerable<GetBriefGenreDto>>(genres);

        var getGenresDto = genresDto as GetBriefGenreDto[] ?? genresDto.ToArray();
        _mapperMock.Setup(m => m.Map<IEnumerable<GetBriefGenreDto>>(genres))
            .Returns(getGenresDto);
        _bookRepoMock.Setup(br => br.GetByIdAsync(bookId, false))!
            .ReturnsAsync(book);

        var genreService = new GenreService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        var returnedGenresDto = await genreService.GetGenresByBookAsync(bookId);
        
        // Assert
        returnedGenresDto.Should().BeEquivalentTo(getGenresDto);
    }
    
    [Fact]
    public async Task GetGenresByBookAsync_ThrowsEntityNotFoundException_IfBookDoesntExist()
    {
        // Arrange
        const int notExistingBookId = -100;

        _bookRepoMock.Setup(gr => gr.GetByIdAsync(notExistingBookId, true))!
            .ReturnsAsync((Book)null!);

        var genreService = new GenreService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await genreService.GetGenresByBookAsync(notExistingBookId));
    }
    
    [Fact]
    public async Task DeleteGenreAsync_ThrowsEntityNotFoundException_IfGenreDoesntExist()
    {
        // Arrange
        const int notExistingBookId = -100;

        _genreRepoMock.Setup(gr => gr.GetByIdAsync(notExistingBookId, true))!
            .ReturnsAsync((Genre)null!);

        var genreService = new GenreService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await genreService.DeleteGenreAsync(notExistingBookId));
    }
    
    [Fact]
    public async Task DeleteGenreAsync_ShouldReturnCallDeleteAndSaveRepoMethods()
    {
        // Arrange
        const int genreId = 10;
        var genre = Builder<Genre>.CreateNew()
            .With(g => g.Id = genreId)
            .Build();

        _genreRepoMock.Setup(gr => gr.GetByIdAsync(genreId, false))!
            .ReturnsAsync(genre);

        var genreService = new GenreService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await genreService.DeleteGenreAsync(genreId);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _genreRepoMock.Verify(gr => gr.DeleteAsync(genre), Times.Once);
    }
}