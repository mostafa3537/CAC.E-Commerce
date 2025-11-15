namespace CAC.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderResponse(
    int Id,
    int CustomerId,
    string CustomerName,
    string CustomerEmail,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status
);

