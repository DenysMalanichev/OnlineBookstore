using AutoMapper;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.PublisherFeatures;
using OnlineBookstore.Persistence.Repositories.Interfaces;

namespace OnlineBookstore.Application.Services.Implementation;

public class PublisherService : IPublisherService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PublisherService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task AddPublisherAsync(CreatePublisherDto createPublisherDto)
    {
        var publisher = _mapper.Map<Publisher>(createPublisherDto);

        await _unitOfWork.PublisherRepository.AddAsync(publisher);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdatePublisherAsync(UpdatePublisherDto updatePublisherDto)
    {
        if (await GetPublisherByIdAsync(updatePublisherDto.Id) is null)
        {
            throw new EntityNotFoundException($"No Publisher with Id '{updatePublisherDto.Id}'");
        }
        
        var publisherToUpdate = _mapper.Map<Publisher>(updatePublisherDto);

        await _unitOfWork.PublisherRepository.UpdateAsync(publisherToUpdate);
        await _unitOfWork.CommitAsync();
    }

    public async Task<GetPublisherDto> GetPublisherByIdAsync(int publisherId)
    {
        var publisher = await _unitOfWork.PublisherRepository.GetByIdAsync(publisherId)!
                   ?? throw new EntityNotFoundException($"No Publisher with Id '{publisherId}'");

        return _mapper.Map<GetPublisherDto>(publisher);
    }

    public async Task<IEnumerable<GetBriefPublisherDto>> GetAllPublishersAsync()
    {
        var publishers = await _unitOfWork.PublisherRepository.GetAllAsync();

        var publishersDto = _mapper.Map<IEnumerable<GetBriefPublisherDto>>(publishers);

        return publishersDto;
    }

    public async Task DeletePublisherAsync(int publisherId)
    {
        var publisherToDelete = await _unitOfWork.PublisherRepository.GetByIdAsync(publisherId)!
                           ?? throw new EntityNotFoundException($"No Publisher with Id '{publisherId}'");

        await _unitOfWork.PublisherRepository.DeleteAsync(publisherToDelete);
        await _unitOfWork.CommitAsync();
    }
}