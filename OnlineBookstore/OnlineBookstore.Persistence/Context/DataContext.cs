using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Persistence.Context;

public class DataContext : IdentityDbContext<User, Role, string>
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }
    
    public virtual DbSet<Genre> Genres { get; set; }
    
    public virtual DbSet<Order> Orders { get; set; }
    
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    
    public virtual DbSet<Publisher> Publishers { get; set; }
}