using AutoMapper;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.BookFeatures;
using OnlineBookstore.Features.Interfaces;

namespace OnlineBookstore.Application.Services.Implementation;

public class BookService : IBookService
{
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

    public async Task DeleteBookAsync(int bookId)
    {
        var bookToDelete = await _unitOfWork.BookRepository.GetByIdAsync(bookId)!
            ?? throw new EntityNotFoundException($"No Book with Id '{bookId}'");

        await _unitOfWork.BookRepository.DeleteAsync(bookToDelete);
        await _unitOfWork.CommitAsync();
    }
}