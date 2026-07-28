namespace ERP.Domain.Identity.Users;

using System.Collections.Generic;
using ERP.Domain.Identity.Roles;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;
using ERP.Domain.Shared.ValueObjects;

public class User : BaseEntity
{
    private readonly List<RefreshToken> _refreshTokens = [];

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public int RoleId { get; private set; }

    public Role? Role { get; private set; }
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User() { }

    public User(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        int roleId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new BusinessRuleValidationException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new BusinessRuleValidationException("Last name is required.");

        if (roleId <= 0)
            throw new BusinessRuleValidationException("Role ID must be valid.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = new Email(email);
        PasswordHash = passwordHash;
        RoleId = roleId;
    }

    public void UpdateProfile(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new BusinessRuleValidationException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new BusinessRuleValidationException("Last name is required.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = new Email(email);
    }

    public void AssignRole(int roleId)
    {
        if (roleId <= 0)
            throw new BusinessRuleValidationException("Role ID must be valid.");

        RoleId = roleId;
    }
}
