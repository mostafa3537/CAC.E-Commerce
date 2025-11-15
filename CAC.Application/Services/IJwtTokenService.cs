using CAC.Domain.Entities;

namespace CAC.Application.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    bool ValidateToken(string token, out int userId, out string role);
}

