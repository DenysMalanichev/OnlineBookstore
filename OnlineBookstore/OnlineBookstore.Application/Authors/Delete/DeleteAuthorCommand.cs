using MediatR;

namespace OnlineBookstore.Application.Author.Delete;

public class DeleteAuthorCommand : IRequest
{
    public int AuthorId { get; set; }
}