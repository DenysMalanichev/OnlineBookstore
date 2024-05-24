using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.OrderDetails.Update;

public class UpdateOrderDetailCommandHandler : IRequestHandler<UpdateOrderDetailCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateOrderDetailCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(UpdateOrderDetailCommand request, CancellationToken cancellationToken)
    {
        var orderDetail = _mapper.Map<OrderDetail>(request);

        await _unitOfWork.OrderDetailRepository.UpdateAsync(request.Id, orderDetail);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}