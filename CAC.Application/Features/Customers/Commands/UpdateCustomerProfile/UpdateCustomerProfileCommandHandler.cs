using CAC.Infrastrucure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CAC.Application.Features.Customers.Commands.UpdateCustomerProfile;

public class UpdateCustomerProfileCommandHandler : IRequestHandler<UpdateCustomerProfileCommand, UpdateCustomerProfileResponse>
{
    private readonly ApplicationDbContext _context;

    public UpdateCustomerProfileCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateCustomerProfileResponse> Handle(UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.CustomerId, cancellationToken);

        if (customer == null)
            throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");

        // Check if email is already taken by another user
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != request.CustomerId, cancellationToken);

        if (emailExists)
            throw new InvalidOperationException($"Email '{request.Email}' is already taken by another user.");

        customer.Name = request.Name;
        customer.Email = request.Email;

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateCustomerProfileResponse(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.CreatedDate
        );
    }
}

