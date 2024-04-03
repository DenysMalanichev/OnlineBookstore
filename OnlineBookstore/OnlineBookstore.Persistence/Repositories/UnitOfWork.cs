using OnlineBookstore.Features.Interfaces;
using OnlineBookstore.Persistence.Context;
using OnlineBookstore.Persistence.Repositories.RepoImplementations;

namespace OnlineBookstore.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _dataContext;

    private IAuthorRepository _authorRepository = null!;
    private IBookRepository _bookRepository = null!;
    private ICommentRepository _commentRepository = null!;
    private IGenreRepository _genreRepository = null!;
    private IOrderDetailRepository _orderDetailRepository = null!;
    private IOrderRepository _orderRepository = null!;
    private IPublisherRepository _publisherRepository = null!;

    public UnitOfWork(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public IAuthorRepository AuthorRepository 
        => _authorRepository ??= new AuthorRepository(_dataContext);
    
    public IBookRepository BookRepository 
        => _bookRepository ??= new BookRepository(_dataContext);
    
    public ICommentRepository CommentRepository 
        => _commentRepository ??= new CommentRepository(_dataContext);
    
    public IGenreRepository GenreRepository 
        => _genreRepository ??= new GenreRepository(_dataContext);
    
    public IOrderRepository OrderRepository
        => _orderRepository ??= new OrderRepository(_dataContext);
    
    public IOrderDetailRepository OrderDetailRepository 
        => _orderDetailRepository ??= new OrderDetailRepository(_dataContext);
    
    public IPublisherRepository PublisherRepository 
        => _publisherRepository ??= new PublisherRepository(_dataContext);
    
    public async Task CommitAsync()
    {
        await _dataContext.SaveChangesAsync();
    }
}