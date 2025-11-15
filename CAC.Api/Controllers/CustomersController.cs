using MediatR;
using Microsoft.AspNetCore.Mvc;
using CAC.Application.Features.Customers.Queries.GetCustomerProfile;
using CAC.Application.Features.Customers.Commands.UpdateCustomerProfile;

namespace CAC.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<ActionResult<CustomerProfileDto>> GetProfile([FromQuery] int customerId)
    {
        var result = await _mediator.Send(new GetCustomerProfileQuery(customerId));
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UpdateCustomerProfileResponse>> UpdateProfile(
        [FromQuery] int customerId,
        [FromBody] UpdateCustomerProfileCommand command)
    {
        if (customerId != command.CustomerId)
            return BadRequest("Customer ID mismatch.");

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

