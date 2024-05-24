using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Publishers.Update;

public class UpdatePublisherCommandHandler : IRequestHandler<UpdatePublisherCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdatePublisherCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task Handle(UpdatePublisherCommand request, CancellationToken cancellationToken)
    {
        
        var publisherToUpdate = _mapper.Map<Publisher>(request);

        await _unitOfWork.PublisherRepository.UpdateAsync(request.Id, publisherToUpdate);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}