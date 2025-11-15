using CAC.Domain.Entities;
using CAC.Domain.Enums;
using CAC.Infrastrucure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CAC.Application.Features.Orders.Commands.PlaceOrder;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, PlaceOrderResponse>
{
    private readonly ApplicationDbContext _context;

    public PlaceOrderCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PlaceOrderResponse> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate customer exists
        var customer = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.CustomerId, cancellationToken);

        if (customer == null)
            throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");

        // Validate products exist and get them with stock information
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .ToListAsync(cancellationToken);

        if (products.Count != productIds.Count)
        {
            var missingIds = productIds.Except(products.Select(p => p.Id)).ToList();
            throw new KeyNotFoundException($"Products with IDs {string.Join(", ", missingIds)} not found or inactive.");
        }

        // Validate stock availability and calculate total
        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);

            // Validate stock availability
            if (product.StockQuantity < item.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {item.Quantity}.");
            }

            // Calculate item total
            var itemTotal = product.Price * item.Quantity;
            totalAmount += itemTotal;

            // Reduce stock quantity
            product.ReduceStock(item.Quantity);

            // Create order item
            var orderItem = OrderItem.Create(
                product.Id,
                item.Quantity,
                product.Price,
                product
            );

            orderItems.Add(orderItem);
        }

        // Create order
        var order = new Order
        {
            CustomerId = request.CustomerId,
            Customer = customer,
            OrderDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            OrderItems = orderItems
        };

        // Set order reference for each order item
        foreach (var orderItem in orderItems)
        {
            orderItem.SetOrder(order);
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        // Load order with items for response
        await _context.Entry(order)
            .Collection(o => o.OrderItems)
            .Query()
            .Include(oi => oi.Product)
            .ThenInclude(p => p.Category)
            .LoadAsync(cancellationToken);

        return new PlaceOrderResponse(
            order.Id,
            order.CustomerId,
            order.Customer.Name,
            order.Customer.Email,
            order.OrderDate,
            order.TotalAmount,
            order.Status.ToString(),
            order.OrderItems.Select(oi => new OrderItemResponseDto(
                oi.Id,
                oi.ProductId,
                oi.Product.Name,
                oi.Quantity,
                oi.PriceAtOrder
            )).ToList()
        );
    }
}

