namespace CAC.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenResponse(
    int Id,
    string Name,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken
);

