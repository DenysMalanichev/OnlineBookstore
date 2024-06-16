using MediatR;
using OnlineBookstore.Application.Comments.Dtos;

namespace OnlineBookstore.Application.Comments.GetById;

public class GetCommentByIdQuery : IRequest<GetCommentDto>
{
    public int CommentId { get; set; }
}