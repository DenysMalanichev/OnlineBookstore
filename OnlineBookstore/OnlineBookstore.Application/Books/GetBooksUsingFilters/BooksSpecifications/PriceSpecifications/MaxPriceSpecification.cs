using System.Linq.Expressions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Books.GetBooksUsingFilters.BooksSpecifications.PriceSpecifications;

public class MaxPriceSpecification : ISpecification<Book>
{
    private readonly decimal _maxPrice;

    public MaxPriceSpecification(decimal maxPrice)
    {
        _maxPrice = maxPrice;
    }

    public Expression<Func<Book, bool>> Criteria =>
        book => _maxPrice >= book.Price;
}