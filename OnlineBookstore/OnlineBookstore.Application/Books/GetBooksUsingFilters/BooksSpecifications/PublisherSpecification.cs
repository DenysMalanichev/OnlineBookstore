using System.Linq.Expressions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Books.GetBooksUsingFilters.BooksSpecifications;

public class PublisherSpecification : ISpecification<Book>
{
    private readonly int _publishersId;

    public PublisherSpecification(int publishersId)
    {
        _publishersId = publishersId;
    }

    public Expression<Func<Book, bool>> Criteria =>
        book => _publishersId == book.Publisher.Id;
}