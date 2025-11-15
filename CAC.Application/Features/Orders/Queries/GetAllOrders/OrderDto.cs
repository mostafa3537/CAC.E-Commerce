namespace CAC.Application.Features.Orders.Queries.GetAllOrders;

public record OrderDto(
    int Id,
    int CustomerId,
    string CustomerName,
    string CustomerEmail,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status,
    List<OrderItemDto> Items
);

public record OrderItemDto(
    int Id,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal PriceAtOrder
);

