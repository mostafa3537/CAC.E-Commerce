using CAC.Application.Common.Helpers;
using CAC.Application.Common.Models;
using CAC.Infrastrucure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CAC.Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, PagedResult<OrderDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetAllOrdersQueryHandler> _logger;

    public GetAllOrdersQueryHandler(
        ApplicationDbContext context,
        ILogger<GetAllOrdersQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all orders with pagination. Page: {PageNumber}, PageSize: {PageSize}",
            request.PageNumber, request.PageSize);

        var query = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.Category)
            .AsQueryable();

        // Apply sorting (default to OrderDate descending)
        query = QueryHelper.ApplySorting(query, request.SortBy ?? "OrderDate", request.SortDirection);

        var totalCount = await query.CountAsync(cancellationToken);

        var orders = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var result = new PagedResult<OrderDto>
        {
            Items = orders.Select(o => new OrderDto(
                o.Id,
                o.CustomerId,
                o.Customer.Name,
                o.Customer.Email,
                o.OrderDate,
                o.TotalAmount,
                o.Status.ToString(),
                o.OrderItems.Select(oi => new OrderItemDto(
                    oi.Id,
                    oi.ProductId,
                    oi.Product.Name,
                    oi.Quantity,
                    oi.PriceAtOrder
                )).ToList()
            )).ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        _logger.LogInformation("Retrieved {Count} orders from database", result.Items.Count);

        return result;
    }
}

