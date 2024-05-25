using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore.Query;
using OnlineBookstore.Application.Books;
using OnlineBookstore.Application.Books.Dtos;
using OnlineBookstore.Application.Books.GetAvgBookRating;
using OnlineBookstore.Application.Books.GetBooksByAuthor;
using OnlineBookstore.Application.Books.GetBooksByPublisher;
using OnlineBookstore.Application.Books.GetBooksUsingFilters;
using OnlineBookstore.Application.Books.GetById;

namespace OnlineBookstore.CoreService.Tests;

public class BookServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IBookQueryRepository> _bookRepoMock = new();
    private readonly IMapper _mapper;

    public BookServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GetBookDto, Book>().ReverseMap();
            cfg.CreateMap<GetBriefBookDto, Book>().ReverseMap();
        });
        _mapper = config.CreateMapper();
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

        var bookService = new GetBookByIdQueryHandler(_bookRepoMock.Object, _mapperMock.Object);

        // Act
        var returnedBook = await bookService.Handle(new GetBookByIdQuery {BookId = existingBookId },
            CancellationToken.None);

        // Assert
        returnedBook.Should().BeEquivalentTo(expectedBookDto);
    }
    
    [Fact]
    public async Task GetBooksByAuthor_ShouldReturnCorrectPagesResultByAuthor()
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

        var bookService = new GetBooksByAuthorQueryHandler(_bookRepoMock.Object, _mapperMock.Object);

        // Act
        var returnedBook = await bookService.Handle(
            new GetBooksByAuthorQuery
            {
                AuthorId = authorId, 
                Page = page, 
                ItemsOnPage = 10
            }, 
            CancellationToken.None);

        // Assert
        returnedBook.Entities.Should().BeEquivalentTo(getBriefBookDto);
        Assert.Equal(2, returnedBook.CurrentPage);
        Assert.Equal(21, returnedBook.TotalPages);
    }
    
    [Fact]
    public async Task GetBooksByPublisher_ShouldReturnCorrectPagesResultByPublisher()
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

        var bookService = new GetBooksByPublisherQueryHandler(_bookRepoMock.Object, _mapperMock.Object);

        // Act
        var returnedBook = await bookService.Handle(
            new GetBooksByPublisherQuery
            {
                PublisherId = publisherId, 
                Page = page, 
                ItemsOnPage = 10
            }, 
            CancellationToken.None);

        // Assert
        returnedBook.Entities.Should().BeEquivalentTo(getBriefBookDto);
        Assert.Equal(2, returnedBook.CurrentPage);
        Assert.Equal(21, returnedBook.TotalPages);
    }

    [Fact]
    public async Task CountAvgRatingOfBook_ShouldReturnCountAsInDb()
    {
        // Arrange
        const int bookId = 100;
        const int expectedCount = 10;

        _bookRepoMock.Setup(br => br.CountAvgRatingForBook(bookId))
            .Returns(expectedCount);

        var bookService = new GetAvgBookRatingQueryHandler(_bookRepoMock.Object);

        // Act
        var returnedCount = await bookService.Handle(
            new GetAvgBookRatingQuery { BookId = bookId }, CancellationToken.None);
        
        // Assert
        Assert.Equal(expectedCount, returnedCount);
    }

    [Theory]
    [MemberData(nameof(ValidNotNullPriceBoundsData))]
    [MemberData(nameof(ValidNullPriceBoundsData))]
    [MemberData(nameof(ValidMaxPriceNullData))]
    [MemberData(nameof(ValidMinPriceNullData))]
    public async Task GetBooksUsingFiltersAsync_ReturnsCorrectPagingInfo(GetFilteredBooksQuery filteredBooksQuery)
    {
        // Arrange
        var books = Enumerable.Range(1, 21).Select(n => new Book { Id = n }).AsQueryable();
        var pagedBooks = books.Skip(10).Take(10).ToList();
        
        _bookRepoMock.Setup(r => r.GetItemsByPredicate(It.IsAny<ExpressionStarter<Book>>(), false))
                .Returns(AsyncQueryableFactory.CreateAsyncQueryable(books));
        _mapperMock.Setup(m => m.Map<IEnumerable<GetBriefBookDto>>(It.IsAny<IList<Book>>()))
            .Returns(pagedBooks.Select(b => new GetBriefBookDto { Id = b.Id }).ToList());

        var service = new GetFilteredBooksQueryHandler(_bookRepoMock.Object, _mapperMock.Object);

        // Act
        var result = await service.Handle(filteredBooksQuery, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.CurrentPage);
        Assert.Equal(10, result.Entities.Count());
    }

    [Theory]
    [MemberData(nameof(InvalidPriceBoundsData))]
    public async Task GetBooksUsingFiltersAsync_ThrowArgumentException_IfMinPriceIsGraterThanMaxPrice(
        GetFilteredBooksQuery filteredBooksQuery)
    {
        // Arrange
        var books = Enumerable.Range(1, 21).Select(n => new Book { Id = n }).AsQueryable();
        var pagedBooks = books.Skip(10).Take(10).ToList();

        _bookRepoMock.Setup(r => r.GetItemsByPredicate(It.IsAny<ExpressionStarter<Book>>(), false))
            .Returns(AsyncQueryableFactory.CreateAsyncQueryable(books));
        _mapperMock.Setup(m => m.Map<IEnumerable<GetBriefBookDto>>(It.IsAny<IList<Book>>()))
            .Returns(pagedBooks.Select(b => new GetBriefBookDto { Id = b.Id }).ToList());

        var service = new GetFilteredBooksQueryHandler(_bookRepoMock.Object, _mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.Handle(filteredBooksQuery, CancellationToken.None));
    }

    public static IEnumerable<object[]> ValidNotNullPriceBoundsData()
    {
        yield return new object[]
        {
            new GetFilteredBooksQuery
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
            new GetFilteredBooksQuery
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
            new GetFilteredBooksQuery
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
            new GetFilteredBooksQuery
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
                new GetFilteredBooksQuery
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