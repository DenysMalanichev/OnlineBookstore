using OnlineBookstore.Features.CommentFeatures;
using OnlineBookstore.Features.GenreFeatures;

namespace OnlineBookstore.Application.Services.Interfaces;

public interface ICommentService
{
    Task AddCommentAsync(CreateCommentDto createCommentDto, string userId);

    Task<GetCommentDto> GetCommentByIdAsync(int commentId);
    
    Task<IEnumerable<GetCommentDto>> GetCommentsByBookIdAsync(int bookId);
}