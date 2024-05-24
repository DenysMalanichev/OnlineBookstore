using MediatR;

namespace OnlineBookstore.Application.Publishers.Delete;

public class DeletePublisherCommand: IRequest
{
    public int PublisherId { get; set; }
}