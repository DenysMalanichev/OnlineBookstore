using AutoMapper;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.Interfaces;
using OnlineBookstore.Features.OrderFeatures.OrderDetailFeatures;

namespace OnlineBookstore.Application.Services.Implementation;

public class OrderDetailService : IOrderDetailService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task AddOrderDetailAsync(AddOrderDetailDto addOrderDetailDto)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(addOrderDetailDto.OrderId)!
                    ?? throw new EntityNotFoundException($"No Order opened with Id '{addOrderDetailDto.OrderId}'");

        if (order.OrderStatus == OrderStatus.Closed)
        {
            throw new OrderClosedException("Order with Id '{addOrderDetailDto.OrderId}' is already closed.");
        }

        var orderDetail = _mapper.Map<OrderDetail>(addOrderDetailDto);
        orderDetail.Order = order;

        await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
        await _unitOfWork.CommitAsync();
    }

    public async Task<GetOrderDetailDto> GetOrderDetailByIdAsync(int orderDetailId)
    {
        var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(orderDetailId)!
                          ?? throw new EntityNotFoundException($"No Order Detail with Id '{orderDetailId}'");

        var orderDetailDto = _mapper.Map<GetOrderDetailDto>(orderDetail);

        return orderDetailDto;
    }

    public async Task UpdateOrderDetailAsync(UpdateOrderDetailDto updateOrderDetailDto)
    {
        var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(updateOrderDetailDto.Id)!
                          ?? throw new EntityNotFoundException($"No Order Detail with Id '{updateOrderDetailDto.Id}'");
        orderDetail = _mapper.Map<OrderDetail>(updateOrderDetailDto);

        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteOrderDetailAsync(int orderDetailId)
    {
        var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(orderDetailId)!
                          ?? throw new EntityNotFoundException($"No Order Detail with Id '{orderDetailId}'");

        await _unitOfWork.OrderDetailRepository.DeleteAsync(orderDetail);

        await _unitOfWork.CommitAsync();
    }
}