using CAC.Application.Features.Orders.Queries.GetAllOrders;
using MediatR;

namespace CAC.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(int Id) : IRequest<OrderDto?>;

