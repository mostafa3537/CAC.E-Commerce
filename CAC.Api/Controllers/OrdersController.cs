using MediatR;
using Microsoft.AspNetCore.Mvc;
using CAC.Application.Features.Orders.Commands.PlaceOrder;
using CAC.Application.Features.Orders.Commands.CancelOrder;

namespace CAC.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<PlaceOrderResponse>> PlaceOrder([FromBody] PlaceOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtRoute(
            routeName: null,
            routeValues: new { id = result.Id },
            value: result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<CancelOrderResponse>> CancelOrder(
        int id,
        [FromQuery] int customerId)
    {
        var result = await _mediator.Send(new CancelOrderCommand(id, customerId));
        return Ok(result);
    }
}

