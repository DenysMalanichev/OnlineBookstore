using System.Linq.Expressions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Books.GetBooksUsingFilters.BooksSpecifications;

public class AuthorNameSpecification : ISpecification<Book>
{
    private readonly string _nameBeginning;

    public AuthorNameSpecification(string nameBeginning)
    {
        _nameBeginning = nameBeginning;
    }

    public Expression<Func<Book, bool>> Criteria =>
        book => (book.Author.FirstName + book.Author.LastName).StartsWith(_nameBeginning);
}