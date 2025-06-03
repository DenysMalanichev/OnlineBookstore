using System.ComponentModel.DataAnnotations;
using MediatR;

namespace OnlineBookstore.Application.Comments.Create;

public class CreateCommentCommand : IRequest
{
    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    [Range(1, 5)]
    public int BookRating { get; set; }

    public int BookId { get; set; }

    public string UserId { get; set; } = null!;
}