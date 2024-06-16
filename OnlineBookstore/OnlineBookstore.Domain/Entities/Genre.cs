using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Domain.Entities;

public class Genre : IBaseEntity
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual IList<Book> Books { get; set; } = null!;
    
    public int Id { get; set; }
}