using OnlineBookstore.Application.Author;
using OnlineBookstore.Application.Books;
using OnlineBookstore.Application.Comments;
using OnlineBookstore.Application.Genres;
using OnlineBookstore.Application.OrderDetails;
using OnlineBookstore.Application.Orders;
using OnlineBookstore.Application.Publishers;

namespace OnlineBookstore.Application.Common;

public interface IUnitOfWork
{
    IAuthorCommandRepository AuthorRepository { get; }
    
    IBookCommandRepository BookRepository { get; }
    
    ICommentCommandRepository CommentRepository { get; }
    
    IGenreCommandRepository GenreRepository { get; }
    
    IOrderDetailCommandRepository OrderDetailRepository { get; }
    
    IOrderCommandRepository OrderRepository { get; }
    
    IPublisherCommandRepository PublisherRepository { get; }
    
    Task CommitAsync(CancellationToken cancellationToken = default);
}