using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Features.CommentFeatures;

public class CreateCommentDto
{
    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    [Range(1, 5)]
    public int BookRating { get; set; }

    public int BookId { get; set; }

    public int UserId { get; set; }
}