using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Exceptions;

namespace OnlineBookstore.Application.Publishers.Delete;

public class DeletePublisherCommandHandler : IRequestHandler<DeletePublisherCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DeletePublisherCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task Handle(DeletePublisherCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.PublisherRepository.DeleteAsync(request.PublisherId);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}