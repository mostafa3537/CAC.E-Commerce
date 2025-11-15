using CAC.Application.Services;
using CAC.Infrastrucure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CAC.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(ApplicationDbContext context, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenEntity = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshTokenEntity == null || refreshTokenEntity.IsRevoked || refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        // Revoke old refresh token
        refreshTokenEntity.IsRevoked = true;

        // Generate new tokens
        var accessToken = _jwtTokenService.GenerateToken(refreshTokenEntity.User);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // Save new refresh token
        var newRefreshTokenEntity = new CAC.Domain.Entities.RefreshToken
        {
            UserId = refreshTokenEntity.UserId,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            User = refreshTokenEntity.User
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(
            refreshTokenEntity.User.Id,
            refreshTokenEntity.User.Name,
            refreshTokenEntity.User.Email,
            refreshTokenEntity.User.Role.ToString(),
            accessToken,
            newRefreshToken
        );
    }
}

