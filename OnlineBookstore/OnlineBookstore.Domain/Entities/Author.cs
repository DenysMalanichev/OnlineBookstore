using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Domain.Entities;

public class Author : IBaseEntity
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; } = null!;
    
    public int Id { get; set; }
}