using MediatR;

namespace CAC.Application.Features.Customers.Queries.GetCustomerProfile;

public record GetCustomerProfileQuery(int CustomerId) : IRequest<CustomerProfileDto?>;

public record CustomerProfileDto(
    int Id,
    string Name,
    string Email,
    DateTime CreationDate
);

