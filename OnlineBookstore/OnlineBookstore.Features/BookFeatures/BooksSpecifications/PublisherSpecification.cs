using System.Linq.Expressions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Features.BookFeatures.BooksSpecifications;

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