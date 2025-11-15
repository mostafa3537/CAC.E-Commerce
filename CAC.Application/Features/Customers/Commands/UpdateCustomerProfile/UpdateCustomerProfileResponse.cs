namespace CAC.Application.Features.Customers.Commands.UpdateCustomerProfile;

public record UpdateCustomerProfileResponse(
    int Id,
    string Name,
    string Email,
    DateTime CreatedDate
);

