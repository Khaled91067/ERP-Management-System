namespace ERP.Domain.Identity.Roles;

using System;
using System.Collections.Generic;
using System.Linq;
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

        var trimmedPermissions = permissions?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(trimmedPermissions))
        {
            Permissions = "[]";
            return;
        }

        if (trimmedPermissions.StartsWith('['))
        {
            List<string>? parsed;
            try
            {
                parsed = System.Text.Json.JsonSerializer.Deserialize<List<string>>(trimmedPermissions);
            }
            catch (System.Text.Json.JsonException)
            {
                throw new BusinessRuleValidationException("Permissions must be a valid JSON string array.");
            }

            SetPermissions(parsed ?? []);
            return;
        }

        Permissions = trimmedPermissions;
    }

    public IReadOnlyList<string> GetPermissions()
    {
        if (string.IsNullOrWhiteSpace(Permissions))
            return [];

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(Permissions) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public void SetPermissions(IEnumerable<string> permissions)
    {
        var knownPermissions = Identity.Permissions.Permissions.All;
        var list = (permissions ?? [])
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var perm in list)
        {
            if (!knownPermissions.Contains(perm, StringComparer.OrdinalIgnoreCase))
            {
                throw new BusinessRuleValidationException($"Unknown permission '{perm}'.");
            }
        }

        Permissions = System.Text.Json.JsonSerializer.Serialize(list);
    }
}
