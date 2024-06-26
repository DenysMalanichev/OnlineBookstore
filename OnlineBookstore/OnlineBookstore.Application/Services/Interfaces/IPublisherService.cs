using OnlineBookstore.Features.PublisherFeatures;

namespace OnlineBookstore.Application.Services.Interfaces;

public interface IPublisherService
{
    Task AddPublisherAsync(CreatePublisherDto createPublisherDto);
    
    Task UpdatePublisherAsync(UpdatePublisherDto updatePublisherDto);

    Task<GetPublisherDto> GetPublisherByIdAsync(int publisherId);
    
    Task<IEnumerable<GetBriefPublisherDto>> GetAllPublishersAsync();

    Task DeletePublisherAsync(int publisherId);
}