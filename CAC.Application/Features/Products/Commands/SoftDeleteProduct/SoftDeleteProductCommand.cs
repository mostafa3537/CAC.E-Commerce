using MediatR;

namespace CAC.Application.Features.Products.Commands.SoftDeleteProduct;

public record SoftDeleteProductCommand(int Id) : IRequest<bool>;

