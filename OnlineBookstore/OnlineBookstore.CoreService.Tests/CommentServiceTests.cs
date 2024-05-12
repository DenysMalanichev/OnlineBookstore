using System.Collections;
using OnlineBookstore.Features.CommentFeatures;

namespace OnlineBookstore.CoreService.Tests;

public class CommentServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ICommentRepository> _commentRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;

    public CommentServiceTests()
    {
        _unitOfWorkMock.Setup(uow => uow.CommentRepository)
            .Returns(_commentRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync())
            .Returns(Task.CompletedTask);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateCommentDto, Comment>().ReverseMap();
            cfg.CreateMap<GetCommentDto, Comment>().ReverseMap();
        });
            
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task AddCommentAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var createCommentDto = Builder<CreateCommentDto>.CreateNew().Build();
        var comment = _mapper.Map<Comment>(createCommentDto);

        _commentRepoMock.Setup(cr =>
                cr.IsUserWroteCommentForThisBook(userId, createCommentDto.BookId))
            .Returns(false);
        _mapperMock.Setup(m => m.Map<Comment>(createCommentDto))
            .Returns(comment);
        
        var commentService = new CommentService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await commentService.AddCommentAsync(createCommentDto, userId);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _commentRepoMock.Verify(uow => uow.AddAsync(comment), Times.Once);
    }
    
    [Fact]
    public async Task AddCommentAsync_ThrowUserAlreadyWroteCommentForBookException_IfUserAlreadyWroteReview()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var createCommentDto = Builder<CreateCommentDto>.CreateNew().Build();
        var comment = _mapper.Map<Comment>(createCommentDto);

        _commentRepoMock.Setup(cr =>
                cr.IsUserWroteCommentForThisBook(userId, createCommentDto.BookId))
            .Returns(true);
        _mapperMock.Setup(m => m.Map<Comment>(createCommentDto))
            .Returns(comment);
        
        var commentService = new CommentService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<UserAlreadyWroteCommentForBookException>(async () =>
            await commentService.AddCommentAsync(createCommentDto, userId));
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
        
        var commentService = new CommentService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        var returnedCommentDto = await commentService.GetCommentByIdAsync(commentId);
        
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

        var commentService = new CommentService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await commentService.GetCommentByIdAsync(notExistingCommentId));
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

        var commentService = new CommentService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        var returnedComments = await commentService.GetCommentsByBookIdAsync(bookId);
        
        // Assert
        returnedComments.Should().BeEquivalentTo(getCommentDto);
    }
}