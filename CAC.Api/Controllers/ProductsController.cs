using MediatR;
using Microsoft.AspNetCore.Mvc;
using CAC.Application.Features.Products.Queries.GetAllProducts;
using CAC.Application.Features.Products.Queries.GetProductById;
using CAC.Application.Features.Products.Queries.SearchProducts;
using CAC.Application.Common.Models;

namespace CAC.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetAll(
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDirection = "asc")
    {
        var result = await _mediator.Send(new GetAllProductsQuery(
            includeInactive,
            pageNumber,
            pageSize,
            sortBy,
            sortDirection));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<ProductDto>>> Search(
        [FromQuery] string? name = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDirection = "asc")
    {
        var result = await _mediator.Send(new SearchProductsQuery(
            name,
            pageNumber,
            pageSize,
            sortBy,
            sortDirection));
        return Ok(result);
    }
}

