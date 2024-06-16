using MediatR;
using OnlineBookstore.Features.PublisherFeatures;

namespace OnlineBookstore.Application.Publishers.GetAll;

public class GetAllPublishersQuery : IRequest<IEnumerable<GetBriefPublisherDto>>
{
    
}