using OnlineBookstore.Features.PublisherFeatures;

namespace OnlineBookstore.CoreService.Tests;

public class PublisherServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IPublisherRepository> _publisherRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;
    
    public PublisherServiceTests()
    {
        _unitOfWorkMock.Setup(uow => uow.PublisherRepository)
            .Returns(_publisherRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync())
            .Returns(Task.CompletedTask);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreatePublisherDto, Publisher>().ReverseMap();
            cfg.CreateMap<UpdatePublisherDto, Publisher>().ReverseMap();
            cfg.CreateMap<GetPublisherDto, Publisher>().ReverseMap();
            cfg.CreateMap<GetBriefPublisherDto, Publisher>().ReverseMap();
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task AddPublisherAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        var createPublisherDto = Builder<CreatePublisherDto>.CreateNew().Build();
        var publisher = _mapper.Map<Publisher>(createPublisherDto);

        _publisherRepoMock.Setup(pr => pr.AddAsync(publisher))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<Publisher>(createPublisherDto))
            .Returns(publisher);

        var publisherService = new PublisherService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await publisherService.AddPublisherAsync(createPublisherDto);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _publisherRepoMock.Verify(pr => pr.AddAsync(publisher), Times.Once);
    }
    
    [Fact]
    public async Task UpdatePublisherAsync_ShouldCallUpdateAndSaveRepoMethods()
    {
        // Arrange
        const int publisherId = 100;
        var updatePublisherDto = Builder<UpdatePublisherDto>.CreateNew()
            .With(up => up.Id = publisherId)
            .Build();
        var publisher = _mapper.Map<Publisher>(updatePublisherDto);

        _publisherRepoMock.Setup(pr => pr.GetByIdAsync(publisherId, true))!
            .ReturnsAsync(publisher);
        _publisherRepoMock.Setup(pr => pr.UpdateAsync(publisher))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<Publisher>(updatePublisherDto))
            .Returns(publisher);

        var publisherService = new PublisherService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await publisherService.UpdatePublisherAsync(updatePublisherDto);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _publisherRepoMock.Verify(pr => pr.UpdateAsync(publisher), Times.Once);
    }
    
    [Fact]
    public async Task UpdatePublisherAsync_ThrowsEntityNotFoundException_IfDoesntExist()
    {
        // Arrange
        const int notExistingPublisherId = -100;
        var updatePublisherDto = Builder<UpdatePublisherDto>.CreateNew()
            .With(up => up.Id = notExistingPublisherId)
            .Build();

        _publisherRepoMock.Setup(pr => pr.GetByIdAsync(notExistingPublisherId, true))!
            .ReturnsAsync((Publisher)null!);

        var publisherService = new PublisherService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await publisherService.UpdatePublisherAsync(updatePublisherDto));
    }
    
    [Fact]
    public async Task GetPublisherByIdAsync_ThrowsEntityNotFoundException_IfDoesntExist()
    {
        // Arrange
        const int notExistingPublisherId = -100;

        _publisherRepoMock.Setup(pr => pr.GetByIdAsync(notExistingPublisherId, false))!
            .ReturnsAsync((Publisher)null!);

        var publisherService = new PublisherService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await publisherService.GetPublisherByIdAsync(notExistingPublisherId));
    }
    
    [Fact]
    public async Task GetPublisherByIdAsync_ReturnsPublisherDto_IfExists()
    {
        // Arrange
        const int publisherId = 100;
        var publisher = Builder<Publisher>.CreateNew()
            .With(up => up.Id = publisherId)
            .Build();
        var publisherDto = _mapper.Map<GetPublisherDto>(publisher);

        _publisherRepoMock.Setup(pr => pr.GetByIdAsync(publisherId, false))!
            .ReturnsAsync(publisher);
        _mapperMock.Setup(m => m.Map<GetPublisherDto>(publisher))
            .Returns(publisherDto);

        var publisherService = new PublisherService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        var returnedPublisher = await publisherService.GetPublisherByIdAsync(publisherId);
        
        // Assert
        returnedPublisher.Should().BeEquivalentTo(publisherDto);
    }
    
    [Fact]
    public async Task GetAllPublishersAsync_ReturnsPublishersDto()
    {
        // Arrange
        var publishers = Builder<Publisher>.CreateListOfSize(10).Build();
        var publishersDto = _mapper.Map<List<GetBriefPublisherDto>>(publishers);

        _publisherRepoMock.Setup(pr => pr.GetAllAsync())
            .ReturnsAsync(publishers);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetBriefPublisherDto>>(publishers))
            .Returns(publishersDto);

        var publisherService = new PublisherService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        var returnedPublishers = await publisherService.GetAllPublishersAsync();
        
        // Assert
        returnedPublishers.Should().BeEquivalentTo(publishersDto);
    }
    
    [Fact]
    public async Task DeletePublisherAsync_ThrowsEntityNotFoundException_IfDoesntExist()
    {
        // Arrange
        const int notExistingPublisherId = -100;

        _publisherRepoMock.Setup(pr => pr.GetByIdAsync(notExistingPublisherId, false))!
            .ReturnsAsync((Publisher)null!);

        var publisherService = new PublisherService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await publisherService.DeletePublisherAsync(notExistingPublisherId));
    }
    
    [Fact]
    public async Task DeletePublisherAsync_ShouldCallDeleteAndSaveMethods()
    {
        // Arrange
        const int publisherId = 100;
        var publishers = Builder<Publisher>.CreateNew()
            .With(p => p.Id = publisherId)
            .Build();

        _publisherRepoMock.Setup(pr => pr.GetByIdAsync(publisherId, false))!
            .ReturnsAsync(publishers);

        var publisherService = new PublisherService(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await publisherService.DeletePublisherAsync(publisherId);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _publisherRepoMock.Verify(pr => pr.DeleteAsync(publishers), Times.Once);
    }
}