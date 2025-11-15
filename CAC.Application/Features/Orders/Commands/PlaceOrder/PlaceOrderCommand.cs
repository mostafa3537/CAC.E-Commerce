using MediatR;

namespace CAC.Application.Features.Orders.Commands.PlaceOrder;

public record PlaceOrderCommand(
    int CustomerId,
    List<OrderItemDto> Items
) : IRequest<PlaceOrderResponse>;

public record OrderItemDto(
    int ProductId,
    int Quantity
);

