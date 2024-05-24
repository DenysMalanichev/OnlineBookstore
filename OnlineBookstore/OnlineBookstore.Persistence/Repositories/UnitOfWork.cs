using System.Diagnostics.CodeAnalysis;
using OnlineBookstore.Application.Author;
using OnlineBookstore.Application.Books;
using OnlineBookstore.Application.Comments;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Genres;
using OnlineBookstore.Application.OrderDetails;
using OnlineBookstore.Application.Orders;
using OnlineBookstore.Application.Publishers;
using OnlineBookstore.Persistence.Context;
using OnlineBookstore.Persistence.Repositories.RepoImplementations;

namespace OnlineBookstore.Persistence.Repositories;

[ExcludeFromCodeCoverage]
public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _dataContext;

    private IAuthorCommandRepository _authorRepository = null!;
    private IBookCommandRepository _bookRepository = null!;
    private ICommentCommandRepository _commentRepository = null!;
    private IGenreCommandRepository _genreRepository = null!;
    private IOrderDetailCommandRepository _orderDetailRepository = null!;
    private IOrderCommandRepository _orderRepository = null!;
    private IPublisherCommandRepository _publisherRepository = null!;

    public UnitOfWork(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public IAuthorCommandRepository AuthorRepository 
        => _authorRepository ??= new AuthorCommandRepository(_dataContext);

    public IBookCommandRepository BookRepository 
        => _bookRepository ??= new BookCommandRepository(_dataContext);
    
    public ICommentCommandRepository CommentRepository 
        => _commentRepository ??= new CommentCommandRepository(_dataContext);
    
    public IGenreCommandRepository GenreRepository 
        => _genreRepository ??= new GenreCommandRepository(_dataContext);
    
    public IOrderCommandRepository OrderRepository
        => _orderRepository ??= new OrderCommandRepository(_dataContext);
    
    public IOrderDetailCommandRepository OrderDetailRepository 
        => _orderDetailRepository ??= new OrderDetailCommandRepository(_dataContext);
    
    public IPublisherCommandRepository PublisherRepository 
        => _publisherRepository ??= new PublisherCommandRepository(_dataContext);
    
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _dataContext.SaveChangesAsync(cancellationToken);
    }
}