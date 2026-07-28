namespace ERP.Domain.Identity.Roles;

using System.Collections.Generic;
using ERP.Domain.Identity.Users;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;

public class Role : BaseEntity
{
    private readonly List<User> _users = [];

    public string Name { get; private set; } = string.Empty;

    // Storing as JSON string
    public string Permissions { get; private set; } = string.Empty;

    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private Role() { }

    public Role(string name, string permissions = "")
    {
        UpdateDetails(name, permissions);
    }

    public void UpdateDetails(string name, string permissions = "")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Role name is required.");

        Name = name.Trim();
        Permissions = permissions?.Trim() ?? string.Empty;
    }
}
