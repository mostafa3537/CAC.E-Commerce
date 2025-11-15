using CAC.Domain.Enums;
using CAC.Infrastrucure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CAC.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, UpdateOrderStatusResponse>
{
    private readonly ApplicationDbContext _context;

    public UpdateOrderStatusCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateOrderStatusResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
            throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");

        // Parse status
        if (!Enum.TryParse<OrderStatus>(request.Status, out var newStatus))
            throw new ArgumentException($"Invalid status: {request.Status}");

        // If changing from Pending to Cancelled, restore stock
        if (order.Status == OrderStatus.Pending && newStatus == OrderStatus.Cancelled)
        {
            foreach (var orderItem in order.OrderItems)
            {
                orderItem.Product.RestoreStock(orderItem.Quantity);
            }
        }

        order.Status = newStatus;
        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateOrderStatusResponse(
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

