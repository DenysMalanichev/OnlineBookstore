using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;

namespace OnlineBookstore.Application.OrderDetails.Delete;

public class DeleteOrderDetailCommandHandler : IRequestHandler<DeleteOrderDetailCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DeleteOrderDetailCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task Handle(DeleteOrderDetailCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.OrderDetailRepository.DeleteAsync(request.OrderDetailId);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}