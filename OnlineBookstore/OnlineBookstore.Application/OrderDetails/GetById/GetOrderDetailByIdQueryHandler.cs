using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.OrderDetails.Dtos;

namespace OnlineBookstore.Application.OrderDetails.GetById;

public class GetOrderDetailByIdQueryHandler : IRequestHandler<GetOrderDetailByIdQuery, GetOrderDetailDto>
{
    private readonly IOrderDetailQueryRepository _orderDetailQueryRepository;
    private readonly IMapper _mapper;

    public GetOrderDetailByIdQueryHandler(IOrderDetailQueryRepository orderDetailQueryRepository, IMapper mapper)
    {
        _mapper = mapper;
        _orderDetailQueryRepository = orderDetailQueryRepository;
    }

    public async Task<GetOrderDetailDto> Handle(GetOrderDetailByIdQuery request, CancellationToken cancellationToken)
    {
        var orderDetail = await _orderDetailQueryRepository.GetByIdAsync(request.OrderDetailId)!
                          ?? throw new EntityNotFoundException($"No Order Detail with Id '{request.OrderDetailId}'");

        var orderDetailDto = _mapper.Map<GetOrderDetailDto>(orderDetail);

        return orderDetailDto;
    }
}