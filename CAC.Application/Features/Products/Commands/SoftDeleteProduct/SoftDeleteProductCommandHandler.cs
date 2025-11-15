using MediatR;
using CAC.Infrastrucure;

namespace CAC.Application.Features.Products.Commands.SoftDeleteProduct;

public class SoftDeleteProductCommandHandler : IRequestHandler<SoftDeleteProductCommand, bool>
{
    private readonly ApplicationDbContext _context;

    public SoftDeleteProductCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(SoftDeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);

        if (product == null)
            return false;

        product.SoftDelete();
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

