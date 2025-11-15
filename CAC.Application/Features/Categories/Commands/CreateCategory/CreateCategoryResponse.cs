namespace CAC.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryResponse(int Id, string Name, string Description, DateTime CreatedDate, DateTime UpdatedDate);

