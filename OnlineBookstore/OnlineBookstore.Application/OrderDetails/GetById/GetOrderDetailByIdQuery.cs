using System.Reflection.Metadata.Ecma335;
using MediatR;
using OnlineBookstore.Application.OrderDetails.Dtos;

namespace OnlineBookstore.Application.OrderDetails.GetById;

public class GetOrderDetailByIdQuery : IRequest<GetOrderDetailDto>
{
    public int OrderDetailId { get; set; }
}