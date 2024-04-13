using AutoMapper;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.AuthorFeatures;
using OnlineBookstore.Features.Interfaces;

namespace OnlineBookstore.Application.Services.Implementation;

public class AuthorService : IAuthorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuthorService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task AddAuthorAsync(CreateAuthorDto createAuthorDto)
    {
        var author = _mapper.Map<Author>(createAuthorDto);

        await _unitOfWork.AuthorRepository.AddAsync(author);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto)
    {
        if (await GetAuthorByIdAsync(updateAuthorDto.Id) is null)
        {
            throw new EntityNotFoundException($"No Author with Id '{updateAuthorDto.Id}'");
        }
        
        var authorToUpdate = _mapper.Map<Author>(updateAuthorDto);

        await _unitOfWork.AuthorRepository.UpdateAsync(authorToUpdate);
        await _unitOfWork.CommitAsync();
    }

    public async Task<GetAuthorDto> GetAuthorByIdAsync(int authorDto)
    {
        var genre = await _unitOfWork.AuthorRepository.GetByIdAsync(authorDto)!
                    ?? throw new EntityNotFoundException($"No Author with Id '{authorDto}'");

        return _mapper.Map<GetAuthorDto>(genre);
    }

    public async Task<IEnumerable<GetAuthorDto>> GetAllAuthorsAsync()
    {
        var authors = await _unitOfWork.AuthorRepository.GetAllAsync();

        var authorDtos = _mapper.Map<IEnumerable<GetAuthorDto>>(authors);

        return authorDtos;
    }

    public async Task DeleteAuthorAsync(int authorDto)
    {
        var authorToDelete = await _unitOfWork.AuthorRepository.GetByIdAsync(authorDto)!
                            ?? throw new EntityNotFoundException($"No Author with Id '{authorDto}'");

        await _unitOfWork.AuthorRepository.DeleteAsync(authorToDelete);
        await _unitOfWork.CommitAsync();
    }
}