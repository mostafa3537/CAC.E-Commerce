using CAC.Application.Features.Orders.Queries.GetAllOrders;
using CAC.Infrastrucure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CAC.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly ApplicationDbContext _context;

    public GetOrderByIdQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null)
            return null;

        return new OrderDto(
            order.Id,
            order.CustomerId,
            order.Customer.Name,
            order.Customer.Email,
            order.OrderDate,
            order.TotalAmount,
            order.Status.ToString(),
            order.OrderItems.Select(oi => new OrderItemDto(
                oi.Id,
                oi.ProductId,
                oi.Product.Name,
                oi.Quantity,
                oi.PriceAtOrder
            )).ToList()
        );
    }
}

