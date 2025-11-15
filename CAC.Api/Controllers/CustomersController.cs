using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAC.Application.Features.Customers.Queries.GetCustomerProfile;
using CAC.Application.Features.Customers.Commands.UpdateCustomerProfile;
using System.Security.Claims;

namespace CAC.Api.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Roles = "Customer")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<ActionResult<CustomerProfileDto>> GetProfile()
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _mediator.Send(new GetCustomerProfileQuery(customerId));
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UpdateCustomerProfileResponse>> UpdateProfile(
        [FromBody] UpdateCustomerProfileRequest request)
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var command = new UpdateCustomerProfileCommand(customerId, request.Name, request.Email);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

