using MediatR;

namespace OnlineBookstore.Application.Books.GetAvgBookRating;

public class GetAvgBookRatingQuery : IRequest<double?>
{
    public int BookId { get; set; }
}