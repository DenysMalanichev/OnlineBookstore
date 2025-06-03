using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Orders.Create;
using OnlineBookstore.Application.Orders.Dtos;
using OnlineBookstore.Domain.Constants;

namespace OnlineBookstore.Application.Orders.GetUserActiveOrder;

public class GetUserActiveOrderQueryHandler : IRequestHandler<GetUserActiveOrderQuery, GetOrderDto>
{
    private readonly IOrderQueryRepository _orderQueryRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetUserActiveOrderQueryHandler(IOrderQueryRepository orderQueryRepository, IMapper mapper, IMediator mediator)
    {
        _orderQueryRepository = orderQueryRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<GetOrderDto> Handle(GetUserActiveOrderQuery request, CancellationToken cancellationToken)
    {
        if ((await _orderQueryRepository.GetUserOrdersAsync(request.UserId))
                .FirstOrDefault(o => o.OrderStatus == OrderStatus.Open) is null)
        {
            await _mediator.Send(new CreateOrderCommand { UserId = request.UserId }, cancellationToken);
        }

        var order = (await _orderQueryRepository.GetUserOrdersAsync(request.UserId))
            .FirstOrDefault(o => o.OrderStatus == OrderStatus.Open);
        
        var orderDto = _mapper.Map<GetOrderDto>(order);
        return orderDto;
    }
}