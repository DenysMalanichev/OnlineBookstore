using AutoMapper;
using Microsoft.Identity.Client;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.GenreFeatures;
using OnlineBookstore.Features.Interfaces;

namespace OnlineBookstore.Application.Services.Implementation;

public class GenreService : IGenreService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GenreService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task AddGenreAsync(CreateGenreDto createGenreDto)
    {
        var genre = _mapper.Map<Genre>(createGenreDto);

        await _unitOfWork.GenreRepository.AddAsync(genre);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateGenreAsync(UpdateGenreDto updateGenreDto)
    {
        if (await GetGenreByIdAsync(updateGenreDto.Id) is null)
        {
            throw new EntityNotFoundException($"No Genre with Id '{updateGenreDto.Id}'");
        }
        
        var genreToUpdate = _mapper.Map<Genre>(updateGenreDto);

        await _unitOfWork.GenreRepository.UpdateAsync(genreToUpdate);
        await _unitOfWork.CommitAsync();
    }

    public async Task<GetGenreDto> GetGenreByIdAsync(int genreId)
    {
        var genre = await _unitOfWork.GenreRepository.GetByIdAsync(genreId)!
                   ?? throw new EntityNotFoundException($"No Genre with Id '{genreId}'");

        return _mapper.Map<GetGenreDto>(genre);
    }

    public async Task DeleteGenreAsync(int genreId)
    {
        var genreToDelete = await _unitOfWork.GenreRepository.GetByIdAsync(genreId)!
            ?? throw new EntityNotFoundException($"No Genre with Id '{genreId}'");

        await _unitOfWork.GenreRepository.DeleteAsync(genreToDelete);
        await _unitOfWork.CommitAsync();
    }
}