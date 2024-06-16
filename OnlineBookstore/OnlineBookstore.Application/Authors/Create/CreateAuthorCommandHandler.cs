using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;

namespace OnlineBookstore.Application.Author.Create;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateAuthorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = _mapper.Map<Domain.Entities.Author>(request);

        await _unitOfWork.AuthorRepository.AddAsync(author);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}