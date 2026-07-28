namespace ERP.Domain.Identity.Users;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Abstractions;

using ERP.Domain.Identity.Roles;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int RoleId { get; set; }

    public Role? Role { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
