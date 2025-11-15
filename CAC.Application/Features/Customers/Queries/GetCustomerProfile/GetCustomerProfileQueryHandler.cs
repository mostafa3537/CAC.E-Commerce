using CAC.Infrastrucure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CAC.Application.Features.Customers.Queries.GetCustomerProfile;

public class GetCustomerProfileQueryHandler : IRequestHandler<GetCustomerProfileQuery, CustomerProfileDto?>
{
    private readonly ApplicationDbContext _context;

    public GetCustomerProfileQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerProfileDto?> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        var customer = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.CustomerId, cancellationToken);

        if (customer == null)
            return null;

        return new CustomerProfileDto(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.CreatedDate
        );
    }
}

