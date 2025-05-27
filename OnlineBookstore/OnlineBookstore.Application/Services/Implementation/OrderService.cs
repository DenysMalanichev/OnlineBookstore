using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.Messages;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.OrderFeatures;
using OnlineBookstore.Persistence.Repositories.Interfaces;
using OnlineBookstore.Persistence.Repositories.RepoImplementations;

namespace OnlineBookstore.Application.Services.Implementation;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly IMapper _mapper;

    public OrderService(
        IKafkaProducerService kafkaProducer,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
        _kafkaProducer = kafkaProducer;
    }


    public async Task<GetOrderDto> GetUsersActiveOrderAsync(string userId)
    {
        var order = (await _unitOfWork.OrderRepository
                .GetUserOrdersAsync(userId))
            .FirstOrDefault(o => o.OrderStatus == OrderStatus.Open)
            ?? await CreateNewOrderAsync(userId);

        var orderDto = _mapper.Map<GetOrderDto>(order);
        return orderDto;
    }

    public async Task<IEnumerable<GetOrderDto>> GetUsersOrdersAsync(string userId)
    {
        var orders = await _unitOfWork.OrderRepository.GetUserOrdersAsync(userId);

        var orderDtos = _mapper.Map<IEnumerable<GetOrderDto>>(orders);

        return orderDtos;
    }

    public async Task CloseUsersOrderAsync(CreateOrderDto createOrderDto, string userId)
    {
        var order = (await _unitOfWork.OrderRepository
                        .GetUserOrdersAsync(userId))
                    .FirstOrDefault(o => o.OrderStatus == OrderStatus.Open)
                    ?? throw new EntityNotFoundException("No Order was open to proceed.");

        order.ShipAddress = createOrderDto.ShipAddress;
        order.ShipCity = createOrderDto.ShipCity;
        order.OrderStatus = OrderStatus.Closed;
        order.OrderClosed = DateTime.UtcNow;

        await _unitOfWork.CommitAsync();

        foreach(var orderDetail in order.OrderDetails)
        {
            await _kafkaProducer.ProduceAsync<string, BookPurchasedMessage>(
                "recommendations.book-purchased",
                orderDetail.BookId.ToString(),
                new BookPurchasedMessage { BookId = orderDetail.BookId, UserId = userId });
        }           
    }

    private async Task<Order> CreateNewOrderAsync(string userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId)
                   ?? throw new EntityNotFoundException($"User with Id '{userId}' not found.");

        var newOrder = new Order
        {
            UserId = userId,
            OrderStatus = OrderStatus.Open,
        };

        await _unitOfWork.OrderRepository.AddAsync(newOrder);
        await _unitOfWork.CommitAsync();

        return newOrder;
    }

    public async Task<IEnumerable<BooksOrdersStatisticsDto>> GetBooksOrderStatisticsAsync(int bookId)
    {
        var yearOfInterest = DateTime.UtcNow.Year;

        var booksOrderDetails = (await _unitOfWork.OrderDetailRepository.GetAllAsync())
            .Where(od => od.BookId == bookId);

        var statisticsArray = (await _unitOfWork.OrderRepository.GetOrdersByYearAsync(
            booksOrderDetails.Select(od => od.OrderId).ToArray(), yearOfInterest))
            .GroupBy(o => o.OrderClosed!.Value.Month);

        return statisticsArray.Select(orderGroup => 
            new BooksOrdersStatisticsDto
            {
                Quantity = orderGroup.Sum(o => o.OrderDetails.Where(od => od.BookId == bookId).Sum(od => od.Quantity)),
                Month = orderGroup.First().OrderClosed!.Value.Month,
            });
    }
}