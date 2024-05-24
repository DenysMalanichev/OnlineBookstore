using AutoMapper;
using MediatR;
using OnlineBookstore.Application.OrderDetails.Dtos;
using OnlineBookstore.Application.Orders.Dtos;

namespace OnlineBookstore.Application.Orders.GetUserOrders;

public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, IEnumerable<GetOrderDto>>
{
    private readonly IOrderQueryRepository _orderQueryRepository;
    private readonly IMapper _mapper;

    public GetUserOrdersQueryHandler(IOrderQueryRepository orderQueryRepository, IMapper mapper)
    {
        _orderQueryRepository = orderQueryRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<GetOrderDto>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderQueryRepository.GetUserOrdersAsync(request.UserId);

        var orderDtos = _mapper.Map<IEnumerable<GetOrderDto>>(orders);

        return orderDtos;
    }
}