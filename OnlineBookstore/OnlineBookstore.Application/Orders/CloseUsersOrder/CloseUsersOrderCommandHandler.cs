using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Orders.GetUserActiveOrder;

namespace OnlineBookstore.Application.Orders.CloseUsersOrder;

public class CloseUsersOrderCommandHandler : IRequestHandler<CloseUsersOrderCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CloseUsersOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediator = mediator;
    }
    
    public async Task Handle(CloseUsersOrderCommand request, CancellationToken cancellationToken)
    {
        var activeOrder = await _mediator.Send(
            new GetUserActiveOrderQuery { UserId = request.UserId }, cancellationToken);
        
        var closeOrderData = _mapper.Map<CloseOrderData>(request);
        closeOrderData.OrderId = activeOrder.Id;

        await _unitOfWork.OrderRepository.CloseOrderAsync(closeOrderData);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}