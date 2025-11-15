using CAC.Infrastrucure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CAC.Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderDto>>
{
    private readonly ApplicationDbContext _context;

    public GetAllOrdersQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.Category)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);

        return orders.Select(o => new OrderDto(
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
        )).ToList();
    }
}

