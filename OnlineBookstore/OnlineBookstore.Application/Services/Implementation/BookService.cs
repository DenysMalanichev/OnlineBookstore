using AutoMapper;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineBookstore.Application.Exceptions;
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
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BookService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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
    }

    public async Task UpdateBookAsync(UpdateBookDto updateBookDto)
    {
        if (await _unitOfWork.BookRepository.GetByIdAsync(updateBookDto.Id, true)! is null)
        {
            throw new EntityNotFoundException($"No Book with Id '{updateBookDto.Id}'");
        }
        
        var bookToUpdate = _mapper.Map<Book>(updateBookDto);

        bookToUpdate.Genres = new List<Genre>();
        foreach (var genreId in updateBookDto.GenreIds)
        {
            var genre = await _unitOfWork.GenreRepository.GetByIdAsync(genreId)!
                ?? throw new EntityNotFoundException($"No Genre with Id '{genreId}'");
            
            bookToUpdate.Genres.Add(genre);
        }
        
        bookToUpdate.Publisher = await _unitOfWork.PublisherRepository.GetByIdAsync(updateBookDto.PublisherId)!
                                 ?? throw new EntityNotFoundException($"No Publisher with Id '{updateBookDto.PublisherId}'");
        
        bookToUpdate.Author = await _unitOfWork.AuthorRepository.GetByIdAsync(updateBookDto.AuthorId)!
                              ?? throw new EntityNotFoundException($"No Author with Id '{updateBookDto.AuthorId}'");

        await _unitOfWork.BookRepository.UpdateAsync(bookToUpdate);
        await _unitOfWork.CommitAsync();
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

        var entitiesQuery = _unitOfWork.BookRepository.GetItemsByPredicate(predicate, filteredBooksDto.IsDescending ?? false);

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
        var books = _unitOfWork.BookRepository.GetBooksByAuthorAsync(authorId, page ?? 1, itemsOnPage);

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

            specifications.Add(new PriceSpecification((decimal)filteredBooksDto.MinPrice, (decimal)filteredBooksDto.MaxPrice));
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
}