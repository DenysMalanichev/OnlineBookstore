using System.ComponentModel.DataAnnotations;
using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Domain.Entities;

public class Comment : BaseEntity
{
    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    [Range(1, 5)]
    public int BookRating { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}