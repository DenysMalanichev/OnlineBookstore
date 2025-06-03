using System.Linq.Expressions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Books.GetBooksUsingFilters.BooksSpecifications;

public class NameSpecification : ISpecification<Book>
{
    private readonly string _nameBeginning;

    public NameSpecification(string nameBeginning)
    {
        _nameBeginning = nameBeginning;
    }

    public Expression<Func<Book, bool>> Criteria =>
        book => book.Name.StartsWith(_nameBeginning);
}