using System.Linq.Expressions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Features.BookFeatures.BooksSpecifications.PriceSpecifications;

public class PriceSpecification : ISpecification<Book>
{
    private readonly decimal _minPrice;
    private readonly decimal _maxPrice;

    public PriceSpecification(decimal minPrice, decimal maxPrice)
    {
        _minPrice = minPrice;
        _maxPrice = maxPrice;
    }

    public Expression<Func<Book, bool>> Criteria =>
        book => _minPrice <= book.Price && book.Price <= _maxPrice;
}