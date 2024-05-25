using OnlineBookstore.Application.Books;
using OnlineBookstore.Application.Books.Create;
using OnlineBookstore.Application.Books.Delete;
using OnlineBookstore.Application.Books.Dtos;
using OnlineBookstore.Application.Books.Update;
using OnlineBookstore.Application.Common;

namespace OnlineBookstore.CoreService.Tests;

public class BookCommandsTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IBookCommandRepository> _bookRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Faker _faker = new();
    private readonly IMapper _mapper;

    public BookCommandsTests()
    {
        _unitOfWorkMock.Setup(uow => uow.BookRepository)
            .Returns(_bookRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.GenreRepository);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync(CancellationToken.None))
            .Returns(Task.CompletedTask);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateBookCommand, Book>().ReverseMap();
            cfg.CreateMap<UpdateBookCommand, Book>();
            cfg.CreateMap<GetBookDto, Book>().ReverseMap();
            cfg.CreateMap<GetBriefBookDto, Book>().ReverseMap();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task AddBookAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        var createBookDto = Builder<CreateBookCommand>.CreateNew()
            .With(b => b.GenreIds = _faker.Make(10, () => _faker.Random.Int(1, 100000)))
            .Build();
        var expectedBook = _mapper.Map<Book>(createBookDto);

        _mapperMock.Setup(m => m.Map<Book>(createBookDto))
            .Returns(expectedBook);
        _bookRepoMock.Setup(br => br.AddAsync(expectedBook))
            .Returns(Task.CompletedTask);

        var bookService = new CreateBookCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act
        await bookService.Handle(createBookDto, CancellationToken.None);

        // Assert
        _bookRepoMock.Verify(br => br.AddAsync(expectedBook), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateBookAsync_ShouldCallUpdateAndSaveRepoMethods()
    {
        // Arrange
        var updateBookDto = Builder<UpdateBookCommand>.CreateNew()
            .With(b => b.GenreIds = _faker.Make(10, () => _faker.Random.Int(1, 100000)))
            .Build();
        var expectedBook = _mapper.Map<Book>(updateBookDto);

        _mapperMock.Setup(m => m.Map<Book>(updateBookDto))
            .Returns(expectedBook);
        _bookRepoMock.Setup(br => br.UpdateAsync(expectedBook))
            .Returns(Task.CompletedTask);

        var bookService = new UpdateBookCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act
        await bookService.Handle(updateBookDto, CancellationToken.None);

        // Assert
        _bookRepoMock.Verify(br => br.UpdateAsync(expectedBook), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteBookAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        const int bookId = 100;
        var book = Builder<Book>.CreateNew()
            .With(b => b.Id = bookId)
            .Build();

        var bookService = new DeleteBookCommandHandler(_unitOfWorkMock.Object);

        // Act
        await bookService.Handle(new DeleteBookCommand { BookId = bookId}, CancellationToken.None);

        // Assert
        _bookRepoMock.Verify(br => br.DeleteAsync(book.Id), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
    }
}