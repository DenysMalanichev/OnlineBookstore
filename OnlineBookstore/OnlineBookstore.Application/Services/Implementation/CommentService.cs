using AutoMapper;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.CommentFeatures;
using OnlineBookstore.Features.Interfaces;

namespace OnlineBookstore.Application.Services.Implementation;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task AddCommentAsync(CreateCommentDto createGenreDto)
    {
        var comment = _mapper.Map<Comment>(createGenreDto);

        await _unitOfWork.CommentRepository.AddAsync(comment);
        await _unitOfWork.CommitAsync();
    }

    public async Task<GetCommentDto> GetCommentByIdAsync(int commentId)
    {
        var comment = await _unitOfWork.CommentRepository.GetByIdAsync(commentId)!
            ?? throw new EntityNotFoundException($"No Comment with Id '{commentId}'");

        var commentDto = _mapper.Map<GetCommentDto>(comment);

        return commentDto;
    }

    public async Task<IEnumerable<GetCommentDto>> GetCommentsByBookIdAsync(int bookId)
    {
        var comments = await _unitOfWork.CommentRepository.GetCommentsByBookIdAsync(bookId);
        return _mapper.Map<IEnumerable<GetCommentDto>>(comments);
    }
}