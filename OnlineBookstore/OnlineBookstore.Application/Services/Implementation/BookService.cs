using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.Messages;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.BookFeatures;
using OnlineBookstore.Features.BookFeatures.BooksSpecifications;
using OnlineBookstore.Features.BookFeatures.BooksSpecifications.PriceSpecifications;
using OnlineBookstore.Features.Paging;
using OnlineBookstore.Persistence.Repositories.Interfaces;

namespace OnlineBookstore.Application.Services.Implementation;

public class BookService : IBookService
{
    private const int DefaultBooksOnPage = 10;

    private readonly IKafkaProducerService _kafkaProducer;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BookService(IKafkaProducerService kafkaProducer, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _kafkaProducer = kafkaProducer;
    }

    public async Task AddBookAsync(CreateBookDto createBookDto)
    {
        var book = _mapper.Map<Book>(createBookDto);

        book.Genres = new List<Genre>();
        foreach (var genreId in createBookDto.GenreIds)
        {
            var genre = await _unitOfWork.GenreRepository.GetByIdAsync(genreId)!
                        ?? throw new EntityNotFoundException($"No Genre with Id '{genreId}'");

            book.Genres.Add(genre);
        }

        book.Publisher = await _unitOfWork.PublisherRepository.GetByIdAsync(createBookDto.PublisherId)!
                         ?? throw new EntityNotFoundException($"No Publisher with Id '{createBookDto.PublisherId}'");

        book.Author = await _unitOfWork.AuthorRepository.GetByIdAsync(createBookDto.AuthorId)!
                      ?? throw new EntityNotFoundException($"No Author with Id '{createBookDto.AuthorId}'");

        await _unitOfWork.BookRepository.AddAsync(book);
        await _unitOfWork.CommitAsync();

        await _kafkaProducer.ProduceAsync<string, BookUpsertedMessage>(
            "recommendations.book-upserted",
            book.Id.ToString(),
            CreateBookUpsertedMessage(book, purchases: 0));
    }

    public async Task UpdateBookAsync(UpdateBookDto updateBookDto)
    {
        var book = await _unitOfWork.BookRepository.GetByIdAsync(updateBookDto.Id, false)!
            ?? throw new EntityNotFoundException($"No Book with Id '{updateBookDto.Id}'");
        _mapper.Map(updateBookDto, book);

        book.Genres.Clear();
        foreach (var genreId in updateBookDto.GenreIds)
        {
            var genre = await _unitOfWork.GenreRepository.GetByIdAsync(genreId)!
                        ?? throw new EntityNotFoundException($"No Genre with Id '{genreId}'");

            book.Genres.Add(genre);
        }

        book.Publisher = await _unitOfWork.PublisherRepository.GetByIdAsync(updateBookDto.PublisherId)!
                                 ?? throw new EntityNotFoundException(
                                     $"No Publisher with Id '{updateBookDto.PublisherId}'");

        book.Author = await _unitOfWork.AuthorRepository.GetByIdAsync(updateBookDto.AuthorId)!
                              ?? throw new EntityNotFoundException($"No Author with Id '{updateBookDto.AuthorId}'");

        await _unitOfWork.CommitAsync();

        await _kafkaProducer.ProduceAsync<string, BookUpsertedMessage>(
            "recommendations.book-upserted",
            book.Id.ToString(),
            CreateBookUpsertedMessage(book, purchases: 0));
    }

    public async Task<GenericPagingDto<GetBriefBookDto>> GetRecommendationsAsync(Guid userId, int? page, int itemsOnPage = 10)
    {
        var httpClient = new HttpClient();
        var path = $"https://localhost:7235/api/recommendations?userId={userId}&pageNumber={page ?? 1}&pageSize={itemsOnPage}";
        var result = await httpClient.GetAsync(new Uri(path));

        if(result is not null && result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var recommendationData = await new StreamReader(result.Content.ReadAsStream()).ReadToEndAsync();
            var recommendedBookIds = JsonConvert.DeserializeObject<int[]>(recommendationData)!;
            var books = await _unitOfWork.BookRepository.GetByIdAsync(recommendedBookIds, page, itemsOnPage)!;

            var briefBookDtos = _mapper.Map<IEnumerable<GetBriefBookDto>>(books);
            return new GenericPagingDto<GetBriefBookDto>
            {
                CurrentPage = page ?? 1,
                Entities = briefBookDtos,
                TotalPages = (await _unitOfWork.BookRepository.GetAllAsync()).Count(),
            };
        }

        throw new HttpRequestException("Error requesting reccomendations");
    }

    public async Task<GetBookDto> GetBookByIdAsync(int bookId)
    {
        var book = await _unitOfWork.BookRepository.GetByIdAsync(bookId)!
                   ?? throw new EntityNotFoundException($"No Book with Id '{bookId}'");

        return _mapper.Map<GetBookDto>(book);
    }

    public async Task<GenericPagingDto<GetBriefBookDto>> GetBooksUsingFiltersAsync(GetFilteredBooksDto filteredBooksDto)
    {
        var predicate = GenerateFilteringPredicate(filteredBooksDto);

        var entitiesQuery =
            _unitOfWork.BookRepository.GetItemsByPredicate(predicate, filteredBooksDto.IsDescending ?? false);

        var itemsOnPage = filteredBooksDto.ItemsOnPage ?? DefaultBooksOnPage;

        var entities = await entitiesQuery.Skip(((filteredBooksDto.Page ?? 1) - 1) * itemsOnPage)
            .Take(itemsOnPage)
            .ToListAsync();

        var entityDtos = _mapper.Map<IEnumerable<GetBriefBookDto>>(entities);

        var totalPages = entitiesQuery.Count();

        return new GenericPagingDto<GetBriefBookDto>
        {
            CurrentPage = filteredBooksDto.Page ?? 1,
            Entities = entityDtos,
            TotalPages = (totalPages / itemsOnPage) + (totalPages % itemsOnPage > 0 ? 1 : 0),
        };
    }

    public GenericPagingDto<GetBriefBookDto> GetBooksByAuthor(int authorId, int? page, int itemsOnPage = 10)
    {
        var books = _unitOfWork.BookRepository.GetBooksByAuthor(authorId, page ?? 1, itemsOnPage);

        var bookDtos = _mapper.Map<IEnumerable<GetBriefBookDto>>(books.booksOnPage);

        return new GenericPagingDto<GetBriefBookDto>
        {
            CurrentPage = page ?? 1,
            Entities = bookDtos,
            TotalPages = books.totalItems,
        };
    }

    public GenericPagingDto<GetBriefBookDto> GetBooksByPublisher(int publisherId, int? page, int itemsOnPage = 10)
    {
        var books = _unitOfWork.BookRepository.GetBooksByPublisher(publisherId, page ?? 1, itemsOnPage);

        var bookDtos = _mapper.Map<IEnumerable<GetBriefBookDto>>(books.booksOnPage);

        return new GenericPagingDto<GetBriefBookDto>
        {
            CurrentPage = page ?? 1,
            Entities = bookDtos,
            TotalPages = books.totalItems,
        };
    }

    public async Task DeleteBookAsync(int bookId)
    {
        var bookToDelete = await _unitOfWork.BookRepository.GetByIdAsync(bookId)!
                           ?? throw new EntityNotFoundException($"No Book with Id '{bookId}'");

        await _unitOfWork.BookRepository.DeleteAsync(bookToDelete);
        await _unitOfWork.CommitAsync();

        await _kafkaProducer.ProduceAsync<string, BookDeletedMessage>(
            "recommendations.book-purchased",
            bookId.ToString(),
            new BookDeletedMessage { BookId = bookId });
    }

    public double? CountAvgRatingOfBook(int bookId)
    {
        return _unitOfWork.BookRepository.CountAvgRatingForBook(bookId);
    }

    private static ExpressionStarter<Book> GenerateFilteringPredicate(GetFilteredBooksDto filteredBooksDto)
    {
        var specifications = new List<ISpecification<Book>>();

        if (filteredBooksDto.Genres is not null)
        {
            specifications.Add(new GenresSpecification(filteredBooksDto.Genres));
        }

        if (filteredBooksDto.PublisherId is not null)
        {
            specifications.Add(new PublisherSpecification((int)filteredBooksDto.PublisherId));
        }

        if (!filteredBooksDto.Name.IsNullOrEmpty() && filteredBooksDto.Name!.Length >= 3)
        {
            specifications.Add(new NameSpecification(filteredBooksDto.Name));
        }

        if (!filteredBooksDto.AuthorName.IsNullOrEmpty() && filteredBooksDto.AuthorName!.Length >= 3)
        {
            specifications.Add(new AuthorNameSpecification(filteredBooksDto.AuthorName));
        }

        if (filteredBooksDto.MinPrice is not null && filteredBooksDto.MaxPrice is not null)
        {
            if (filteredBooksDto.MinPrice > filteredBooksDto.MaxPrice)
            {
                throw new ArgumentException("Lower bound of price cannot be bigger than Upper one");
            }

            specifications.Add(new PriceSpecification((decimal)filteredBooksDto.MinPrice,
                (decimal)filteredBooksDto.MaxPrice));
        }
        else
        {
            if (filteredBooksDto.MinPrice is not null)
            {
                specifications.Add(new MinPriceSpecification((decimal)filteredBooksDto.MinPrice));
            }

            if (filteredBooksDto.MaxPrice is not null)
            {
                specifications.Add(new MaxPriceSpecification((decimal)filteredBooksDto.MaxPrice));
            }
        }

        var predicate = PredicateBuilder.New<Book>(true);
        return specifications.Aggregate(
            predicate,
            (current, specification) => current.And(specification.Criteria));
    }

    private BookUpsertedMessage CreateBookUpsertedMessage(Book book, int purchases) =>
        new()
        {
            BookId = book.Id,
            Title = book.Name,
            Language = book.Language,
            AuthorId = book.AuthorId,
            GenreIds = book.GenreIds.ToList(),
            Rating = CountAvgRatingOfBook(book.Id) ?? 0.0,
            PurchaseNumber = purchases,
            IsPaperback = book.IsPaperback,
        };

    public async Task SetBookImageAsync(IFormFile image, int bookId)
    {
        if (image == null || image.Length == 0)
        {
            throw new ArgumentException("No image provided or image is empty");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Unsupported file type");
        }

        byte[] imageData;
        using (var memoryStream = new MemoryStream())
        {
            await image.CopyToAsync(memoryStream);
            imageData = memoryStream.ToArray();
        }

        await _unitOfWork.BookRepository.SetBookImageAsync(imageData, bookId);
        await _unitOfWork.CommitAsync();
    }

    public async Task<byte[]?> GetBookImageAsync(int bookId)
    {
        return await _unitOfWork.BookRepository.GetBookImageAsync(bookId);
    }
}