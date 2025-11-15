using MediatR;

namespace CAC.Application.Features.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(
    int OrderId,
    string Status
) : IRequest<UpdateOrderStatusResponse>;

