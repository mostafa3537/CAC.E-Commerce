using MediatR;

namespace CAC.Application.Features.Orders.Queries.GetAllOrders;

public record GetAllOrdersQuery() : IRequest<List<OrderDto>>;

