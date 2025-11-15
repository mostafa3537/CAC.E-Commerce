using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAC.Application.Features.Orders.Commands.PlaceOrder;
using CAC.Application.Features.Orders.Commands.CancelOrder;
using System.Security.Claims;

namespace CAC.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize(Roles = "Customer")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<PlaceOrderResponse>> PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var command = new PlaceOrderCommand(customerId, request.Items);
        var result = await _mediator.Send(command);
        return CreatedAtRoute(
            routeName: null,
            routeValues: new { id = result.Id },
            value: result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<CancelOrderResponse>> CancelOrder(int id)
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _mediator.Send(new CancelOrderCommand(id, customerId));
        return Ok(result);
    }
}

