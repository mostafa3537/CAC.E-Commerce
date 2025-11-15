namespace CAC.Application.Features.Orders.Commands.PlaceOrder;

public record PlaceOrderResponse(
    int Id,
    int CustomerId,
    string CustomerName,
    string CustomerEmail,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status,
    List<OrderItemResponseDto> Items
);

public record OrderItemResponseDto(
    int Id,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal PriceAtOrder
);

