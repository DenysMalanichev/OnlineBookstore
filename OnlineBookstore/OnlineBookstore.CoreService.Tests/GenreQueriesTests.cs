using OnlineBookstore.Application.Genres;
using OnlineBookstore.Application.Genres.Dtos;
using OnlineBookstore.Application.Genres.GetAll;
using OnlineBookstore.Application.Genres.GetByBook;
using OnlineBookstore.Application.Genres.GetById;

namespace OnlineBookstore.CoreService.Tests;

public class GenreQueriesTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IGenreQueryRepository> _genreRepoMock = new();
    private readonly IMapper _mapper;

    public GenreQueriesTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GetGenreDto, Genre>().ReverseMap();
            cfg.CreateMap<GetBriefGenreDto, Genre>().ReverseMap();
        });
            
        _mapper = config.CreateMapper();
    }
    
    [Fact]
    public async Task GetGenreByIdAsync_ThrowsEntityNotFoundException_IfGenreDoesntExist()
    {
        // Arrange
        const int notExistingGenreId = -100;

        _genreRepoMock.Setup(gr => gr.GetByIdAsync(notExistingGenreId, true))!
            .ReturnsAsync((Genre)null!);

        var genreService = new GetGenreByIdQueryHandler(_genreRepoMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await genreService.Handle(
                new GetGenreByIdQuery { GenreId = notExistingGenreId}, CancellationToken.None));
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

        var genreService = new GetGenreByIdQueryHandler(_genreRepoMock.Object, _mapperMock.Object);
        
        // Act
        var returnedGenreDto = await genreService.Handle(
            new GetGenreByIdQuery { GenreId = genreId}, CancellationToken.None);
        
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

        var genreService = new GetAllGenresQueryHandler(_genreRepoMock.Object, _mapperMock.Object);
        
        // Act
        var returnedGenresDto = await genreService.Handle(
            new GetAllGenresQuery(), CancellationToken.None);
        
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
            .With(b => b.Id = bookId)
            .Build();
        var genresDto = _mapper.Map<IEnumerable<GetBriefGenreDto>>(genres);

        var getGenresDto = genresDto as GetBriefGenreDto[] ?? genresDto.ToArray();
        _mapperMock.Setup(m => m.Map<IEnumerable<GetBriefGenreDto>>(genres))
            .Returns(getGenresDto);
        _genreRepoMock.Setup(gr => gr.GetGenresByBookAsync(bookId))
            .ReturnsAsync(genres);

        var genreService = new GetGenresByBookQueryHandler(_genreRepoMock.Object, _mapperMock.Object);
        
        // Act
        var returnedGenresDto = await genreService.Handle(
            new GetGenresByBookQuery { BookId = bookId }, CancellationToken.None);
        
        // Assert
        returnedGenresDto.Should().BeEquivalentTo(getGenresDto);
    }
}