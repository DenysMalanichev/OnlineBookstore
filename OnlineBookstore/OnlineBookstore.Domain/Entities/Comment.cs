using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Domain.Entities;

public class Comment : IBaseEntity
{
    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    [Range(1, 5)]
    public int BookRating { get; set; }

    public virtual Book Book { get; set; } = null!;

    [ForeignKey(nameof(Book))] public int BookId { get; set; }

    public virtual User User { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    
    public int Id { get; set; }
}