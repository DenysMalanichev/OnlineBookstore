using MediatR;
using OnlineBookstore.Application.Comments.Dtos;

namespace OnlineBookstore.Application.Comments.GetByBook;

public class GetCommentsByBookQuery : IRequest<IEnumerable<GetCommentDto>>
{
    public int BookId { get; set; }
}