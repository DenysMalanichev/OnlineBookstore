using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore.Query;
using OnlineBookstore.Features.BookFeatures;

namespace OnlineBookstore.CoreService.Tests;

public class BookServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IBookRepository> _bookRepoMock = new();
    private readonly Mock<IGenreRepository> _genreRepoMock = new();
    private readonly Mock<IPublisherRepository> _publisherRepoMock = new();
    private readonly Mock<IAuthorRepository> _authorRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Faker _faker = new();
    private readonly IMapper _mapper;

    public BookServiceTests()
    {
        _unitOfWorkMock.Setup(uow => uow.BookRepository)
            .Returns(_bookRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.GenreRepository)
            .Returns(_genreRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.PublisherRepository)
            .Returns(_publisherRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.AuthorRepository)
            .Returns(_authorRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync())
            .Returns(Task.CompletedTask);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateBookDto, Book>().ReverseMap();
            cfg.CreateMap<UpdateBookDto, Book>();
            cfg.CreateMap<GetBookDto, Book>().ReverseMap();
            cfg.CreateMap<GetBriefBookDto, Book>().ReverseMap();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task AddBookAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        var createBookDto = Builder<CreateBookDto>.CreateNew()
            .With(b => b.GenreIds = _faker.Make(10, () => _faker.Random.Int(1, 100000)))
            .Build();
        var expectedBook = _mapper.Map<Book>(createBookDto);

        _mapperMock.Setup(m => m.Map<Book>(createBookDto))
            .Returns(expectedBook);
        _bookRepoMock.Setup(br => br.AddAsync(expectedBook))
            .Returns(Task.CompletedTask);
        foreach (var genreId in createBookDto.GenreIds)
        {
            _genreRepoMock.Setup(gr => gr.GetByIdAsync(genreId, false))!
                .ReturnsAsync(new Genre());
        }

        _publisherRepoMock.Setup(pr => pr.GetByIdAsync(createBookDto.PublisherId, false))!
            .ReturnsAsync(new Publisher());
        _authorRepoMock.Setup(ar => ar.GetByIdAsync(createBookDto.AuthorId, false))!
            .ReturnsAsync(new Author());

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act
        await bookService.AddBookAsync(createBookDto);

        // Assert
        _bookRepoMock.Verify(br => br.AddAsync(expectedBook), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task AddBookAsync_ThrowEntityNotFoundException_IfOneOfGenresDoesntExist()
    {
        // Arrange
        var createBookDto = Builder<CreateBookDto>.CreateNew()
            .With(b => b.GenreIds = _faker.Make(10, () => _faker.Random.Int(1, 100000)))
            .Build();
        var book = _mapper.Map<Book>(createBookDto);

        _mapperMock.Setup(m => m.Map<Book>(createBookDto))
            .Returns(book);
        _genreRepoMock.Setup(gr => gr.GetByIdAsync(It.IsAny<int>(), false))!
            .ReturnsAsync(new Genre());

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await bookService.AddBookAsync(createBookDto));
    }

    [Fact]
    public async Task AddBookAsync_ThrowEntityNotFoundException_IfPublisherDoesntExist()
    {
        // Arrange
        var createBookDto = Builder<CreateBookDto>.CreateNew()
            .With(b => b.GenreIds = _faker.Make(10, () => _faker.Random.Int(1, 100000)))
            .Build();
        var expectedBook = _mapper.Map<Book>(createBookDto);

        _mapperMock.Setup(m => m.Map<Book>(createBookDto))
            .Returns(expectedBook);
        _bookRepoMock.Setup(br => br.AddAsync(expectedBook))
            .Returns(Task.CompletedTask);
        foreach (var genreId in createBookDto.GenreIds)
        {
            _genreRepoMock.Setup(gr => gr.GetByIdAsync(genreId, false))!
                .ReturnsAsync(new Genre());
        }

        _publisherRepoMock.Setup(pr => pr.GetByIdAsync(It.IsAny<int>(), false))!
            .ReturnsAsync((Publisher)null!);

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await bookService.AddBookAsync(createBookDto));
    }

    [Fact]
    public async Task AddBookAsync_ThrowEntityNotFoundException_IfAuthorDoesntExist()
    {
        // Arrange
        var createBookDto = Builder<CreateBookDto>.CreateNew()
            .With(b => b.GenreIds = _faker.Make(10, () => _faker.Random.Int(1, 100000)))
            .Build();
        var expectedBook = _mapper.Map<Book>(createBookDto);

        _mapperMock.Setup(m => m.Map<Book>(createBookDto))
            .Returns(expectedBook);
        _bookRepoMock.Setup(br => br.AddAsync(expectedBook))
            .Returns(Task.CompletedTask);
        foreach (var genreId in createBookDto.GenreIds)
        {
            _genreRepoMock.Setup(gr => gr.GetByIdAsync(genreId, false))!
                .ReturnsAsync(new Genre());
        }

        _publisherRepoMock.Setup(pr => pr.GetByIdAsync(It.IsAny<int>(), false))!
            .ReturnsAsync(new Publisher());
        _authorRepoMock.Setup(ar => ar.GetByIdAsync(createBookDto.AuthorId, false))!
            .ReturnsAsync((Author)null!);

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await bookService.AddBookAsync(createBookDto));
    }

    [Fact]
    public async Task UpdateBookAsync_ShouldCallUpdateAndSaveRepoMethods()
    {
        // Arrange
        var updateBookDto = Builder<UpdateBookDto>.CreateNew()
            .With(b => b.GenreIds = _faker.Make(10, () => _faker.Random.Int(1, 100000)))
            .Build();
        var expectedBook = _mapper.Map<Book>(updateBookDto);

        _mapperMock.Setup(m => m.Map<Book>(updateBookDto))
            .Returns(expectedBook);
        _bookRepoMock.Setup(br => br.GetByIdAsync(updateBookDto.Id, true))!
            .ReturnsAsync(expectedBook);
        _bookRepoMock.Setup(br => br.UpdateAsync(expectedBook))
            .Returns(Task.CompletedTask);
        foreach (var genreId in updateBookDto.GenreIds)
        {
            _genreRepoMock.Setup(gr => gr.GetByIdAsync(genreId, false))!
                .ReturnsAsync(new Genre());
        }

        _publisherRepoMock.Setup(pr => pr.GetByIdAsync(updateBookDto.PublisherId, false))!
            .ReturnsAsync(new Publisher());
        _authorRepoMock.Setup(ar => ar.GetByIdAsync(updateBookDto.AuthorId, false))!
            .ReturnsAsync(new Author());

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act
        await bookService.UpdateBookAsync(updateBookDto);

        // Assert
        _bookRepoMock.Verify(br => br.UpdateAsync(expectedBook), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateBookAsync_ThrowEntityNotFoundException_IfBookDoesntExist()
    {
        // Arrange
        var updateBookDto = Builder<UpdateBookDto>.CreateNew()
            .With(b => b.GenreIds = _faker.Make(10, () => _faker.Random.Int(1, 100000)))
            .Build();

        _bookRepoMock.Setup(br => br.GetByIdAsync(updateBookDto.Id, true))!
            .ReturnsAsync((Book)null!);

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await bookService.UpdateBookAsync(updateBookDto));
    }

    [Fact]
    public async Task UpdateBookAsync_ThrowEntityNotFoundException_IfOneOfGenresDoesntExist()
    {
        // Arrange
        var updateBookDto = Builder<UpdateBookDto>.CreateNew()
            .With(b => b.GenreIds = _faker.Make(10, () => _faker.Random.Int(1, 100000)))
            .Build();
        var expectedBook = _mapper.Map<Book>(updateBookDto);

        _mapperMock.Setup(m => m.Map<Book>(updateBookDto))
            .Returns(expectedBook);
        _bookRepoMock.Setup(br => br.GetByIdAsync(updateBookDto.Id, true))!
            .ReturnsAsync(expectedBook);
        _genreRepoMock.Setup(gr => gr.GetByIdAsync(It.IsAny<int>(), false))!
            .ReturnsAsync((Genre)null!);

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await bookService.UpdateBookAsync(updateBookDto));
    }

    [Fact]
    public async Task UpdateBookAsync_ThrowEntityNotFoundException_IfPublisherDoesntExist()
    {
        // Arrange
        var updateBookDto = Builder<UpdateBookDto>.CreateNew()
            .With(b => b.GenreIds = _faker.Make(10, () => _faker.Random.Int(1, 100000)))
            .Build();
        var expectedBook = _mapper.Map<Book>(updateBookDto);

        _mapperMock.Setup(m => m.Map<Book>(updateBookDto))
            .Returns(expectedBook);
        _bookRepoMock.Setup(br => br.GetByIdAsync(updateBookDto.Id, true))!
            .ReturnsAsync(expectedBook);
        foreach (var genreId in updateBookDto.GenreIds)
        {
            _genreRepoMock.Setup(gr => gr.GetByIdAsync(genreId, false))!
                .ReturnsAsync(new Genre());
        }

        _publisherRepoMock.Setup(pr => pr.GetByIdAsync(updateBookDto.PublisherId, false))!
            .ReturnsAsync((Publisher)null!);

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await bookService.UpdateBookAsync(updateBookDto));
    }

    [Fact]
    public async Task UpdateBookAsync_ThrowEntityNotFoundException_IfAuthorDoesntExist()
    {
        // Arrange
        var updateBookDto = Builder<UpdateBookDto>.CreateNew()
            .With(b => b.GenreIds = _faker.Make(10, () => _faker.Random.Int(1, 100000)))
            .Build();
        var expectedBook = _mapper.Map<Book>(updateBookDto);

        _mapperMock.Setup(m => m.Map<Book>(updateBookDto))
            .Returns(expectedBook);
        _bookRepoMock.Setup(br => br.GetByIdAsync(updateBookDto.Id, true))!
            .ReturnsAsync(expectedBook);
        foreach (var genreId in updateBookDto.GenreIds)
        {
            _genreRepoMock.Setup(gr => gr.GetByIdAsync(genreId, false))!
                .ReturnsAsync(new Genre());
        }

        _publisherRepoMock.Setup(pr => pr.GetByIdAsync(updateBookDto.PublisherId, false))!
            .ReturnsAsync(new Publisher());
        _authorRepoMock.Setup(ar => ar.GetByIdAsync(updateBookDto.AuthorId, false))!
            .ReturnsAsync((Author)null!);

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await bookService.UpdateBookAsync(updateBookDto));
    }

    [Fact]
    public async Task GetBookByIdAsync_ThrowEntityNotFoundException_IfAuthorDoesntExist()
    {
        // Arrange
        const int existingBookId = 100;
        var book = Builder<Book>.CreateNew().Build();
        var expectedBookDto = _mapper.Map<GetBookDto>(book);

        _bookRepoMock.Setup(br => br.GetByIdAsync(existingBookId, false))!
            .ReturnsAsync(book);
        _mapperMock.Setup(m => m.Map<GetBookDto>(book)).Returns(expectedBookDto);

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act
        var returnedBook = await bookService.GetBookByIdAsync(existingBookId);

        // Assert
        returnedBook.Should().BeEquivalentTo(expectedBookDto);
    }
    
    [Fact]
    public void GetBooksByAuthor_ShouldReturnCorrectPagesResultByAuthor()
    {
        // Arrange
        const int authorId = 100;
        const int page = 2;
        var books = Builder<Book>.CreateListOfSize(21).Build();
        var expectedBookDto = _mapper.Map<IEnumerable<GetBriefBookDto>>(books);

        _bookRepoMock.Setup(br => br.GetBooksByAuthor(authorId, page, 10))
            .Returns((books, 21));
        var getBriefBookDto = expectedBookDto as GetBriefBookDto[] ?? expectedBookDto.ToArray();
        _mapperMock.Setup(m => m.Map<IEnumerable<GetBriefBookDto>>(books))
            .Returns(getBriefBookDto);

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act
        var returnedBook = bookService.GetBooksByAuthor(authorId, page);

        // Assert
        returnedBook.Entities.Should().BeEquivalentTo(getBriefBookDto);
        Assert.Equal(2, returnedBook.CurrentPage);
        Assert.Equal(21, returnedBook.TotalPages);
    }
    
    [Fact]
    public void GetBooksByPublisher_ShouldReturnCorrectPagesResultByPublisher()
    {
        // Arrange
        const int publisherId = 100;
        const int page = 2;
        var books = Builder<Book>.CreateListOfSize(21).Build();
        var expectedBookDto = _mapper.Map<IEnumerable<GetBriefBookDto>>(books);

        _bookRepoMock.Setup(br => br.GetBooksByPublisher(publisherId, page, 10))
            .Returns((books, 21));
        var getBriefBookDto = expectedBookDto as GetBriefBookDto[] ?? expectedBookDto.ToArray();
        _mapperMock.Setup(m => m.Map<IEnumerable<GetBriefBookDto>>(books))
            .Returns(getBriefBookDto);

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act
        var returnedBook = bookService.GetBooksByPublisher(publisherId, page);

        // Assert
        returnedBook.Entities.Should().BeEquivalentTo(getBriefBookDto);
        Assert.Equal(2, returnedBook.CurrentPage);
        Assert.Equal(21, returnedBook.TotalPages);
    }
    
    [Fact]
    public async Task DeleteBookAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        const int bookId = 100;
        var book = Builder<Book>.CreateNew()
            .With(b => b.Id = bookId)
            .Build();

        _bookRepoMock.Setup(br => br.GetByIdAsync(bookId, false))!
            .ReturnsAsync(book);

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act
        await bookService.DeleteBookAsync(bookId);

        // Assert
        _bookRepoMock.Verify(br => br.DeleteAsync(book), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task DeleteBookAsync_ThrowEntityNotFoundException_IfBookDoesntExist()
    {
        // Arrange
        const int notExistingBookId = 100;

        _bookRepoMock.Setup(br => br.GetByIdAsync(notExistingBookId, false))!
            .ReturnsAsync((Book)null!);

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await bookService.DeleteBookAsync(notExistingBookId));
    }


    [Fact]
    public void CountAvgRatingOfBook_ShouldReturnCountAsInDb()
    {
        // Arrange
        const int bookId = 100;
        const int expectedCount = 10;

        _bookRepoMock.Setup(br => br.CountAvgRatingForBook(bookId))
            .Returns(expectedCount);

        var bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act
        var returnedCount = bookService.CountAvgRatingOfBook(bookId);
        
        // Assert
        Assert.Equal(expectedCount, returnedCount);
    }

    [Theory]
    [MemberData(nameof(ValidNotNullPriceBoundsData))]
    [MemberData(nameof(ValidNullPriceBoundsData))]
    [MemberData(nameof(ValidMaxPriceNullData))]
    [MemberData(nameof(ValidMinPriceNullData))]
    public async Task GetBooksUsingFiltersAsync_ReturnsCorrectPagingInfo(GetFilteredBooksDto filteredBooksDto)
    {
        // Arrange
        var books = Enumerable.Range(1, 21).Select(n => new Book { Id = n }).AsQueryable();
        var pagedBooks = books.Skip(10).Take(10).ToList();
        
        _bookRepoMock.Setup(r => r.GetItemsByPredicate(It.IsAny<ExpressionStarter<Book>>(), false))
                .Returns(AsyncQueryableFactory.CreateAsyncQueryable(books));
        _mapperMock.Setup(m => m.Map<IEnumerable<GetBriefBookDto>>(It.IsAny<IList<Book>>()))
            .Returns(pagedBooks.Select(b => new GetBriefBookDto { Id = b.Id }).ToList());

        var service = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act
        var result = await service.GetBooksUsingFiltersAsync(filteredBooksDto);

        // Assert
        Assert.Equal(2, result.CurrentPage);
        Assert.Equal(10, result.Entities.Count());
    }

    [Theory]
    [MemberData(nameof(InvalidPriceBoundsData))]
    public async Task GetBooksUsingFiltersAsync_ThrowArgumentException_IfMinPriceIsGraterThanMaxPrice(
        GetFilteredBooksDto filteredBooksDto)
    {
        // Arrange
        var books = Enumerable.Range(1, 21).Select(n => new Book { Id = n }).AsQueryable();
        var pagedBooks = books.Skip(10).Take(10).ToList();

        _bookRepoMock.Setup(r => r.GetItemsByPredicate(It.IsAny<ExpressionStarter<Book>>(), false))
            .Returns(AsyncQueryableFactory.CreateAsyncQueryable(books));
        _mapperMock.Setup(m => m.Map<IEnumerable<GetBriefBookDto>>(It.IsAny<IList<Book>>()))
            .Returns(pagedBooks.Select(b => new GetBriefBookDto { Id = b.Id }).ToList());

        var service = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.GetBooksUsingFiltersAsync(filteredBooksDto));
    }

    public static IEnumerable<object[]> ValidNotNullPriceBoundsData()
    {
        yield return new object[]
        {
            new GetFilteredBooksDto
            {
                Page = 2,
                ItemsOnPage = 10,
                IsDescending = false,
                MaxPrice = 100000,
                MinPrice = 0,
                AuthorName = "Name",
                Genres = new List<int> { 1, 2, 3 },
                Name = "Name",
                PublisherId = 12
            }
        };
    }
    
    public static IEnumerable<object[]> ValidMaxPriceNullData()
    {
        yield return new object[]
        {
            new GetFilteredBooksDto
            {
                Page = 2,
                ItemsOnPage = 10,
                IsDescending = false,
                MaxPrice = null,
                MinPrice = 10,
                AuthorName = "Name",
                Genres = new List<int> { 1, 2, 3 },
                Name = "Name",
                PublisherId = 12
            }
        };
    }
    
    public static IEnumerable<object[]> ValidMinPriceNullData()
    {
        yield return new object[]
        {
            new GetFilteredBooksDto
            {
                Page = 2,
                ItemsOnPage = 10,
                IsDescending = false,
                MaxPrice = 100,
                MinPrice = null,
                AuthorName = "Name",
                Genres = new List<int> { 1, 2, 3 },
                Name = "Name",
                PublisherId = 12
            }
        };
    }
    
    public static IEnumerable<object[]> ValidNullPriceBoundsData()
    {
        yield return new object[]
        {
            new GetFilteredBooksDto
            {
                Page = 2,
                ItemsOnPage = 10,
                IsDescending = false,
                MaxPrice = 100000,
                MinPrice = 0,
                AuthorName = "Name",
                Genres = new List<int> { 1, 2, 3 },
                Name = "Name",
                PublisherId = 12
            }
        };
    }

    public static IEnumerable<object[]> InvalidPriceBoundsData()
        {
            yield return new object[]
            {
                new GetFilteredBooksDto
                {
                    Page = 2,
                    ItemsOnPage = 10,
                    IsDescending = false,
                    MaxPrice = 0, // MaxPrice is less than MinPrice
                    MinPrice = 10
                }
            };
        }

    private static class AsyncQueryableFactory
    {
        public static IQueryable<T> CreateAsyncQueryable<T>(IEnumerable<T> source)
        {
            var queryable = source.AsQueryable();

            var mockIQueryProvider = new Mock<IQueryProvider>();
            mockIQueryProvider.As<IAsyncQueryProvider>()
                .Setup(x => x.ExecuteAsync<Task<List<T>>>(It.IsAny<Expression>(), It.IsAny<CancellationToken>()))
                .Returns<Expression, CancellationToken>((expression, _) =>
                    Task.FromResult(queryable.Provider.CreateQuery<T>(expression).ToList()));

            mockIQueryProvider.Setup(x => x.CreateQuery<T>(It.IsAny<Expression>()))
                .Returns<Expression>(expression => new TestAsyncEnumerable<T>(expression, mockIQueryProvider.Object));

            return new TestAsyncEnumerable<T>(queryable.Expression, mockIQueryProvider.Object);
        }
    }

    private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        private readonly IQueryProvider _provider;

        public TestAsyncEnumerable(Expression expression, IQueryProvider provider)
            : base(expression)
        {
            _provider = provider;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => _provider;
    }

    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public TestAsyncEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public T Current => _enumerator.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_enumerator.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _enumerator.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}