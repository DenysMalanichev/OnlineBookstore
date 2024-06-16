using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Features.PublisherFeatures;

namespace OnlineBookstore.Application.Publishers.GetById;

public class GetPublisherByIdQueryHandler : IRequestHandler<GetPublisherByIdQuery, GetPublisherDto>
{
    private readonly IPublisherQueryRepository _publisherQueryRepository;
    private readonly IMapper _mapper;

    public GetPublisherByIdQueryHandler(IPublisherQueryRepository publisherQueryRepository, IMapper mapper)
    {
        _publisherQueryRepository = publisherQueryRepository;
        _mapper = mapper;
    }
    
    public async Task<GetPublisherDto> Handle(GetPublisherByIdQuery request, CancellationToken cancellationToken)
    {
        var publisher = await _publisherQueryRepository.GetByIdAsync(request.PublisherId)!
                        ?? throw new EntityNotFoundException($"No Publisher with Id '{request.PublisherId}'");

        return _mapper.Map<GetPublisherDto>(publisher);
    }
}