namespace CAC.Application.Features.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusResponse(
    int Id,
    int CustomerId,
    string CustomerName,
    string CustomerEmail,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status
);

