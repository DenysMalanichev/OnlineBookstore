using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Publishers;
using OnlineBookstore.Application.Publishers.Create;
using OnlineBookstore.Application.Publishers.Delete;
using OnlineBookstore.Application.Publishers.Update;
using OnlineBookstore.Features.PublisherFeatures;

namespace OnlineBookstore.CoreService.Tests;

public class PublisherCommandsTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IPublisherCommandRepository> _publisherRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;
    
    public PublisherCommandsTests()
    {
        _unitOfWorkMock.Setup(uow => uow.PublisherRepository)
            .Returns(_publisherRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync(CancellationToken.None))
            .Returns(Task.CompletedTask);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreatePublisherCommand, Publisher>().ReverseMap();
            cfg.CreateMap<UpdatePublisherCommand, Publisher>().ReverseMap();
        });

        _mapper = config.CreateMapper();
    }
    
    [Fact]
    public async Task AddPublisherAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        var createPublisherDto = Builder<CreatePublisherCommand>.CreateNew().Build();
        var publisher = _mapper.Map<Publisher>(createPublisherDto);

        _publisherRepoMock.Setup(pr => pr.AddAsync(publisher))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<Publisher>(createPublisherDto))
            .Returns(publisher);

        var publisherService = new CreatePublisherCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await publisherService.Handle(createPublisherDto, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
        _publisherRepoMock.Verify(pr => pr.AddAsync(publisher), Times.Once);
    }
    
    [Fact]
    public async Task UpdatePublisherAsync_ShouldCallUpdateAndSaveRepoMethods()
    {
        // Arrange
        const int publisherId = 100;
        var updatePublisherDto = Builder<UpdatePublisherCommand>.CreateNew()
            .With(up => up.Id = publisherId)
            .Build();
        var publisher = _mapper.Map<Publisher>(updatePublisherDto);

        _publisherRepoMock.Setup(pr => pr.UpdateAsync(publisher))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<Publisher>(updatePublisherDto))
            .Returns(publisher);

        var publisherService = new UpdatePublisherCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await publisherService.Handle(updatePublisherDto, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
        _publisherRepoMock.Verify(pr => pr.UpdateAsync(publisher), Times.Once);
    }
    
    [Fact]
    public async Task DeletePublisherAsync_ShouldCallDeleteAndSaveMethods()
    {
        // Arrange
        const int publisherId = 100;

        var publisherService = new DeletePublisherCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await publisherService.Handle(
            new DeletePublisherCommand { PublisherId = publisherId }, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
        _publisherRepoMock.Verify(pr => pr.DeleteAsync(publisherId), Times.Once);
    }
}