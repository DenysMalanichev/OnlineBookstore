using MediatR;

namespace OnlineBookstore.Application.Books.Delete;

public class DeleteBookCommand : IRequest
{
    public int BookId { get; set; }
}