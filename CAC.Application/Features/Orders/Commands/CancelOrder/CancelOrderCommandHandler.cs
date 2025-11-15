using CAC.Domain.Enums;
using CAC.Infrastrucure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CAC.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, CancelOrderResponse>
{
    private readonly ApplicationDbContext _context;

    public CancelOrderCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CancelOrderResponse> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
            throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");

        // Verify the order belongs to the customer
        if (order.CustomerId != request.CustomerId)
            throw new UnauthorizedAccessException("You are not authorized to cancel this order.");

        // Only pending orders can be cancelled
        if (order.Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Only pending orders can be cancelled. Current status: {order.Status}.");

        // Restore stock quantities
        foreach (var orderItem in order.OrderItems)
        {
            orderItem.Product.RestoreStock(orderItem.Quantity);
        }

        // Update order status
        order.Status = OrderStatus.Cancelled;

        await _context.SaveChangesAsync(cancellationToken);

        return new CancelOrderResponse(
            order.Id,
            order.CustomerId,
            order.Customer.Name,
            order.Customer.Email,
            order.OrderDate,
            order.TotalAmount,
            order.Status.ToString()
        );
    }
}

