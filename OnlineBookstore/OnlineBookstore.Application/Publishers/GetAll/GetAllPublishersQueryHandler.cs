using AutoMapper;
using MediatR;
using OnlineBookstore.Features.PublisherFeatures;

namespace OnlineBookstore.Application.Publishers.GetAll;

public class GetAllPublishersQueryHandler : IRequestHandler<GetAllPublishersQuery, IEnumerable<GetBriefPublisherDto>>
{
    private readonly IPublisherQueryRepository _publisherQueryRepository;
    private readonly IMapper _mapper;

    public GetAllPublishersQueryHandler(IPublisherQueryRepository publisherQueryRepository, IMapper mapper)
    {
        _publisherQueryRepository = publisherQueryRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<GetBriefPublisherDto>> Handle(GetAllPublishersQuery request, CancellationToken cancellationToken)
    {
        var publishers = await _publisherQueryRepository.GetAllAsync();

        var publishersDto = _mapper.Map<IEnumerable<GetBriefPublisherDto>>(publishers);

        return publishersDto;
    }
}