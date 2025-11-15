namespace CAC.Application.Features.Auth.Commands.Login;

public record LoginResponse(
    int Id,
    string Name,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken
);

