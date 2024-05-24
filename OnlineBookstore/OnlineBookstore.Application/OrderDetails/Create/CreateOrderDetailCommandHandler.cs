using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Orders.GetUserActiveOrder;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.OrderDetails.Create;

public class CreateOrderDetailCommandHandler : IRequestHandler<CreateOrderDetailCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CreateOrderDetailCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task Handle(CreateOrderDetailCommand request, CancellationToken cancellationToken)
    {
        var order = await _mediator.Send(new GetUserActiveOrderQuery { UserId = request.UserId }, cancellationToken);

        var orderDetail = _mapper.Map<OrderDetail>(request);
        orderDetail.OrderId = order.Id;

        await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}