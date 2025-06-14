using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Features.CommentFeatures;

public class GetCommentDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    public int BookRating { get; set; }

    public User User { get; set; } = null!;
}