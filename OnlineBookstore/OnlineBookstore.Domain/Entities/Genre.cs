using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Domain.Entities;

public class Genre : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual IList<Book> Books { get; set; } = null!;
}