using MediatR;

namespace CAC.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand(
    int OrderId,
    int CustomerId
) : IRequest<CancelOrderResponse>;

