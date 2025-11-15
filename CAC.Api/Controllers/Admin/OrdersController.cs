using MediatR;
using Microsoft.AspNetCore.Mvc;
using CAC.Application.Features.Orders.Queries.GetAllOrders;
using CAC.Application.Features.Orders.Queries.GetOrderById;
using CAC.Application.Features.Orders.Commands.UpdateOrderStatus;

namespace CAC.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllOrdersQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<UpdateOrderStatusResponse>> UpdateStatus(
        int id,
        [FromBody] UpdateOrderStatusRequest request)
    {
        var command = new UpdateOrderStatusCommand(id, request.Status);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

