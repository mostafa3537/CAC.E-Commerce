using FluentValidation;

namespace CAC.Application.Features.Products.Commands.SoftDeleteProduct;

public class SoftDeleteProductCommandValidator : AbstractValidator<SoftDeleteProductCommand>
{
    public SoftDeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Product ID must be greater than 0.");
    }
}

