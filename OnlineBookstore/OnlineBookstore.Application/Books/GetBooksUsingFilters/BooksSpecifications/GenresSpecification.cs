using System.Linq.Expressions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Books.GetBooksUsingFilters.BooksSpecifications;

public class GenresSpecification : ISpecification<Book>
{
    private readonly IEnumerable<int> _genresIds;

    public GenresSpecification(IEnumerable<int> genresIds)
    {
        _genresIds = genresIds;
    }

    public Expression<Func<Book, bool>> Criteria =>
        book => book.Genres.Select(g => g.Id)
            .Any(id => _genresIds.Contains(id));
}