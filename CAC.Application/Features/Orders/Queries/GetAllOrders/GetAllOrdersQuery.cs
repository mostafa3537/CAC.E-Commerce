using CAC.Application.Common.Models;
using MediatR;

namespace CAC.Application.Features.Orders.Queries.GetAllOrders;

public record GetAllOrdersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    string SortDirection = "desc"
) : IRequest<PagedResult<OrderDto>>;

