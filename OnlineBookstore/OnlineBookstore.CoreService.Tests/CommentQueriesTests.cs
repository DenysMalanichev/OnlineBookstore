using OnlineBookstore.Application.Comments;
using OnlineBookstore.Application.Comments.Dtos;
using OnlineBookstore.Application.Comments.GetByBook;
using OnlineBookstore.Application.Comments.GetById;
using OnlineBookstore.Application.Common;

namespace OnlineBookstore.CoreService.Tests;

public class CommentQueriesTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ICommentQueryRepository> _commentRepoMock = new();
    private readonly IMapper _mapper;

    public CommentQueriesTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateCommentDto, Comment>().ReverseMap();
            cfg.CreateMap<GetCommentDto, Comment>().ReverseMap();
        });
            
        _mapper = config.CreateMapper();
    }
    
    [Fact]
    public async Task GetCommentByIdAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        const int commentId = 100;
        var comment = Builder<Comment>.CreateNew()
            .With(c => c.Id = commentId)
            .Build();
        var commentDto = _mapper.Map<GetCommentDto>(comment);

        _commentRepoMock.Setup(cr => cr.GetByIdAsync(commentId, false))!
            .ReturnsAsync(comment);
        _mapperMock.Setup(m => m.Map<GetCommentDto>(comment))
            .Returns(commentDto);
        
        var commentService = new GetCommentByIdQueryHandler(_commentRepoMock.Object, _mapperMock.Object);
        
        // Act
        var returnedCommentDto = await commentService.Handle(
            new GetCommentByIdQuery { CommentId = commentId }, CancellationToken.None);
        
        // Assert
        returnedCommentDto.Should().BeEquivalentTo(commentDto);
    }
    
    [Fact]
    public async Task GetCommentByIdAsync_ThrowEntityNotFoundException_IfCommentDoesntExist()
    {
        // Arrange
        const int notExistingCommentId = 100;

        _commentRepoMock.Setup(cr => cr.GetByIdAsync(notExistingCommentId, false))!
            .ReturnsAsync((Comment)null!);

        var commentService = new GetCommentByIdQueryHandler(_commentRepoMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await commentService.Handle(
                new GetCommentByIdQuery { CommentId = notExistingCommentId }, CancellationToken.None));
    }
    
    [Fact]
    public async Task GetCommentsByBookIdAsync_ShouldReturnIEnumerableOfComments()
    {
        // Arrange
        const int bookId = 100;
        var comments = Builder<Comment>.CreateListOfSize(10).Build();
        var commentsDto = _mapper.Map<IEnumerable<GetCommentDto>>(comments);
        
        _commentRepoMock.Setup(cr => cr.GetCommentsByBookIdAsync(bookId))
            .ReturnsAsync(comments);
        var getCommentDto = commentsDto as GetCommentDto[] ?? commentsDto.ToArray();
        _mapperMock.Setup(m => m.Map<IEnumerable<GetCommentDto>>(comments))
            .Returns(getCommentDto);

        var commentService = new GetCommentsByBookQueryHandler(_commentRepoMock.Object, _mapperMock.Object);
        
        // Act
        var returnedComments = await commentService.Handle(
            new GetCommentsByBookQuery { BookId = bookId}, CancellationToken.None);
        
        // Assert
        returnedComments.Should().BeEquivalentTo(getCommentDto);
    }
}