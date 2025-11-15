using MediatR;
using Microsoft.AspNetCore.Mvc;
using CAC.Application.Features.Categories.Commands.CreateCategory;
using CAC.Application.Features.Categories.Commands.UpdateCategory;
using CAC.Application.Features.Categories.Commands.DeleteCategory;
using CAC.Application.Features.Categories.Queries.GetAllCategories;
using CAC.Application.Features.Categories.Queries.GetCategoryById;

namespace CAC.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateCategoryResponse>> Create([FromBody] CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateCategoryResponse>> Update(int id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch.");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(id));
        if (!result)
            return NotFound();

        return NoContent();
    }
}

