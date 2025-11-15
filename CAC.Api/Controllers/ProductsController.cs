using MediatR;
using Microsoft.AspNetCore.Mvc;
using CAC.Application.Features.Products.Queries.GetAllProducts;
using CAC.Application.Features.Products.Queries.GetProductById;
using CAC.Application.Features.Products.Queries.SearchProducts;

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
    public async Task<ActionResult<List<ProductDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllProductsQuery(false));
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
    public async Task<ActionResult<List<ProductDto>>> Search([FromQuery] string? name)
    {
        var result = await _mediator.Send(new SearchProductsQuery(name));
        return Ok(result);
    }
}

