using System.Linq.Expressions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Features.BookFeatures.BooksSpecifications.PriceSpecifications;

public class MinPriceSpecification : ISpecification<Book>
{
    private readonly decimal _minPrice;

    public MinPriceSpecification(decimal minPrice)
    {
        _minPrice = minPrice;
    }

    public Expression<Func<Book, bool>> Criteria =>
        game => _minPrice <= game.Price;
}