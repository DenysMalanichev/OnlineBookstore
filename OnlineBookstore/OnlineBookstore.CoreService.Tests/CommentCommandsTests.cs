using OnlineBookstore.Application.Comments;
using OnlineBookstore.Application.Comments.Create;
using OnlineBookstore.Application.Comments.Dtos;
using OnlineBookstore.Application.Common;

namespace OnlineBookstore.CoreService.Tests;

public class CommentCommandsTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ICommentCommandRepository> _commentRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;

    public CommentCommandsTests()
    {
        _unitOfWorkMock.Setup(uow => uow.CommentRepository)
            .Returns(_commentRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync(CancellationToken.None))
            .Returns(Task.CompletedTask);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateCommentCommand, Comment>().ReverseMap();
            cfg.CreateMap<GetCommentDto, Comment>().ReverseMap();
        });
            
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task AddCommentAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var createCommentDto = Builder<CreateCommentCommand>.CreateNew().Build();
        var comment = _mapper.Map<Comment>(createCommentDto);

        _commentRepoMock.Setup(cr =>
                cr.IsUserWroteCommentForThisBook(userId, createCommentDto.BookId))
            .Returns(false);
        _mapperMock.Setup(m => m.Map<Comment>(createCommentDto))
            .Returns(comment);
        
        var commentService = new CreateCommentCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await commentService.Handle(createCommentDto, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
        _commentRepoMock.Verify(uow => uow.AddAsync(comment), Times.Once);
    }
    
    [Fact]
    public async Task AddCommentAsync_ThrowUserAlreadyWroteCommentForBookException_IfUserAlreadyWroteReview()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var createCommentDto = Builder<CreateCommentCommand>
            .CreateNew()
            .With(c => c.UserId = userId)
            .Build();
        var comment = _mapper.Map<Comment>(createCommentDto);

        _commentRepoMock.Setup(cr =>
                cr.IsUserWroteCommentForThisBook(userId, createCommentDto.BookId))
            .Returns(true);
        _mapperMock.Setup(m => m.Map<Comment>(createCommentDto))
            .Returns(comment);
        
        var commentService = new CreateCommentCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<UserAlreadyWroteCommentForBookException>(async () =>
            await commentService.Handle(createCommentDto, CancellationToken.None));
    }
}