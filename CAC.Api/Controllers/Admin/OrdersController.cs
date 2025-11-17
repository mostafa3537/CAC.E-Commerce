using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAC.Application.Features.Orders.Queries.GetAllOrders;
using CAC.Application.Features.Orders.Queries.GetOrderById;
using CAC.Application.Features.Orders.Commands.UpdateOrderStatus;
using CAC.Application.Common.Models;

namespace CAC.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDirection = "desc")
    {
        var result = await _mediator.Send(new GetAllOrdersQuery(
            pageNumber,
            pageSize,
            sortBy,
            sortDirection));
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

