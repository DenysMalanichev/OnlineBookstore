using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Author.Update;
using OnlineBookstore.Application.Common;

namespace OnlineBookstore.Application.Authors.Update;

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateAuthorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var authorToUpdate = _mapper.Map<Domain.Entities.Author>(request);

        await _unitOfWork.AuthorRepository.UpdateAsync(authorToUpdate);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}