using MediatR;

namespace CAC.Application.Features.Customers.Commands.UpdateCustomerProfile;

public record UpdateCustomerProfileCommand(
    int CustomerId,
    string Name,
    string Email
) : IRequest<UpdateCustomerProfileResponse>;

public record UpdateCustomerProfileRequest(
    string Name,
    string Email
);

