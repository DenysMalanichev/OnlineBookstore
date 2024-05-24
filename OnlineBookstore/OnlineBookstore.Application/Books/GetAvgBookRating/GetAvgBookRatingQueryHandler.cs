using MediatR;
using OnlineBookstore.Application.Common;

namespace OnlineBookstore.Application.Books.GetAvgBookRating;

public class GetAvgBookRatingQueryHandler : IRequestHandler<GetAvgBookRatingQuery, double?>
{
    private readonly IBookQueryRepository _bookQueryRepository;

    public GetAvgBookRatingQueryHandler(IBookQueryRepository bookQueryRepository)
    {
        _bookQueryRepository = bookQueryRepository;
    }

    public async Task<double?> Handle(GetAvgBookRatingQuery request, CancellationToken cancellationToken)
    {
        return _bookQueryRepository.CountAvgRatingForBook(request.BookId);
    }
}