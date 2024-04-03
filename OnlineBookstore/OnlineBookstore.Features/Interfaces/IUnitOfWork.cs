namespace OnlineBookstore.Features.Interfaces;

public interface IUnitOfWork
{
    IAuthorRepository AuthorRepository { get; }
    
    IBookRepository BookRepository { get; }
    
    ICommentRepository CommentRepository { get; }
    
    IGenreRepository GenreRepository { get; }
    
    IOrderDetailRepository OrderDetailRepository { get; }
    
    IOrderRepository OrderRepository { get; }
    
    IPublisherRepository PublisherRepository { get; }
    
    Task CommitAsync();
}