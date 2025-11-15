namespace CAC.Application.Features.Auth.Commands.Register;

public record RegisterResponse(
    int Id,
    string Name,
    string Email,
    string Role
);

