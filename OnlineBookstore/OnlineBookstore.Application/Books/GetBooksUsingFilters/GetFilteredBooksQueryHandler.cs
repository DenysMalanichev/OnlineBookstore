using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineBookstore.Application.Books.Dtos;
using OnlineBookstore.Application.Books.GetBooksUsingFilters.BooksSpecifications;
using OnlineBookstore.Application.Books.GetBooksUsingFilters.BooksSpecifications.PriceSpecifications;
using OnlineBookstore.Application.Common.Paging;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Books.GetBooksUsingFilters;

public class GetFilteredBooksQueryHandler : IRequestHandler<GetFilteredBooksQuery, GenericPagingDto<GetBriefBookDto>>
{
    private const int DefaultBooksOnPage = 10;
    
    private readonly IBookQueryRepository _bookQueryRepository;
    private readonly IMapper _mapper;

    public GetFilteredBooksQueryHandler(IBookQueryRepository bookQueryRepository, IMapper mapper)
    {
        _bookQueryRepository = bookQueryRepository;
        _mapper = mapper;
    }

    public async Task<GenericPagingDto<GetBriefBookDto>> Handle(
        GetFilteredBooksQuery filteredBooksQuery,
        CancellationToken cancellationToken)
    {
        var predicate = GenerateFilteringPredicate(filteredBooksQuery);

        var entitiesQuery =
            _bookQueryRepository.GetItemsByPredicate(predicate, filteredBooksQuery.IsDescending ?? false);

        var itemsOnPage = filteredBooksQuery.ItemsOnPage ?? DefaultBooksOnPage;

        var entities = await entitiesQuery.Skip(((filteredBooksQuery.Page ?? 1) - 1) * itemsOnPage)
            .Take(itemsOnPage)
            .ToListAsync(cancellationToken);

        var entityDtos = _mapper.Map<IEnumerable<GetBriefBookDto>>(entities);

        var totalPages = entitiesQuery.Count();

        return new GenericPagingDto<GetBriefBookDto>
        {
            CurrentPage = filteredBooksQuery.Page ?? 1,
            Entities = entityDtos,
            TotalPages = (totalPages / itemsOnPage) + (totalPages % itemsOnPage > 0 ? 1 : 0),
        };
    }

    private static ExpressionStarter<Book> GenerateFilteringPredicate(GetFilteredBooksQuery filteredBooksQuery)
    {
        var specifications = new List<ISpecification<Book>>();

        if (filteredBooksQuery.Genres is not null)
        {
            specifications.Add(new GenresSpecification(filteredBooksQuery.Genres));
        }

        if (filteredBooksQuery.PublisherId is not null)
        {
            specifications.Add(new PublisherSpecification((int)filteredBooksQuery.PublisherId));
        }

        if (!filteredBooksQuery.Name.IsNullOrEmpty() && filteredBooksQuery.Name!.Length >= 3)
        {
            specifications.Add(new NameSpecification(filteredBooksQuery.Name));
        }

        if (!filteredBooksQuery.AuthorName.IsNullOrEmpty() && filteredBooksQuery.AuthorName!.Length >= 3)
        {
            specifications.Add(new AuthorNameSpecification(filteredBooksQuery.AuthorName));
        }

        if (filteredBooksQuery.MinPrice is not null && filteredBooksQuery.MaxPrice is not null)
        {
            if (filteredBooksQuery.MinPrice > filteredBooksQuery.MaxPrice)
            {
                throw new ArgumentException("Lower bound of price cannot be bigger than Upper one");
            }

            specifications.Add(new PriceSpecification((decimal)filteredBooksQuery.MinPrice,
                (decimal)filteredBooksQuery.MaxPrice));
        }
        else
        {
            if (filteredBooksQuery.MinPrice is not null)
            {
                specifications.Add(new MinPriceSpecification((decimal)filteredBooksQuery.MinPrice));
            }

            if (filteredBooksQuery.MaxPrice is not null)
            {
                specifications.Add(new MaxPriceSpecification((decimal)filteredBooksQuery.MaxPrice));
            }
        }

        var predicate = PredicateBuilder.New<Book>(true);
        return specifications.Aggregate(
            predicate,
            (current, specification) => current.And(specification.Criteria));
    }
}