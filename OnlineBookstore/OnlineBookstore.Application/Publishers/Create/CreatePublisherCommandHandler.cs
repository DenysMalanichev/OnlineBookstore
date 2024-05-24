using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Publishers.Create;

public class CreatePublisherCommandHandler : IRequestHandler<CreatePublisherCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePublisherCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(CreatePublisherCommand request, CancellationToken cancellationToken)
    {
        var publisher = _mapper.Map<Publisher>(request);

        await _unitOfWork.PublisherRepository.AddAsync(publisher);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}