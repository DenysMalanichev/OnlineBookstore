using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Publishers;
using OnlineBookstore.Application.Publishers.GetAll;
using OnlineBookstore.Application.Publishers.GetById;
using OnlineBookstore.Features.PublisherFeatures;

namespace OnlineBookstore.CoreService.Tests;

public class PublisherServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IPublisherQueryRepository> _publisherRepoMock = new();
    private readonly IMapper _mapper;
    
    public PublisherServiceTests()
    {

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GetPublisherDto, Publisher>().ReverseMap();
            cfg.CreateMap<GetBriefPublisherDto, Publisher>().ReverseMap();
        });

        _mapper = config.CreateMapper();
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

        var publisherService = new GetPublisherByIdQueryHandler(_publisherRepoMock.Object, _mapperMock.Object);
        
        // Act
        var returnedPublisher = await publisherService.Handle(
            new GetPublisherByIdQuery { PublisherId = publisherId }, new CancellationToken());
        
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

        var publisherService = new GetAllPublishersQueryHandler(_publisherRepoMock.Object, _mapperMock.Object);
        
        // Act
        var returnedPublishers = await publisherService.Handle(
            new GetAllPublishersQuery(), CancellationToken.None);
        
        // Assert
        returnedPublishers.Should().BeEquivalentTo(publishersDto);
    }
}