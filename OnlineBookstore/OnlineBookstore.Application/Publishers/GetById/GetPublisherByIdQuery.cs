using MediatR;
using OnlineBookstore.Features.PublisherFeatures;

namespace OnlineBookstore.Application.Publishers.GetById;

public class GetPublisherByIdQuery : IRequest<GetPublisherDto>
{
    public int PublisherId { get; set; }
}