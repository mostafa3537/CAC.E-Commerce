using CAC.Domain.Enums;
using CAC.Domain.Common;

namespace CAC.Domain.Entities;

public class User : AggregateRoot<int>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;  
    public UserRole Role { get; set; }
}

