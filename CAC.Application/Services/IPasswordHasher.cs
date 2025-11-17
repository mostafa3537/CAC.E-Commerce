// This file is kept for backward compatibility
// The interface has been moved to CAC.Domain.Services
namespace CAC.Application.Services;

[Obsolete("Use CAC.Domain.Services.IPasswordHasher instead")]
public interface IPasswordHasher : Domain.Services.IPasswordHasher
{
}

