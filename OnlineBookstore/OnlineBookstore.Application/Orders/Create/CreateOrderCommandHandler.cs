using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Orders.Create;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(UserManager<User> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken) is null)
        {
            throw new EntityNotFoundException($"User with Id '{request.UserId}' not found.");
        }

        var newOrder = new Order
        {
            UserId = request.UserId,
            OrderStatus = OrderStatus.Open,
        };

        await _unitOfWork.OrderRepository.AddAsync(newOrder);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}