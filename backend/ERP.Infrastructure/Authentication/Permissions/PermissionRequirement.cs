namespace ERP.Infrastructure.Authentication.Permissions;

using Microsoft.AspNetCore.Authorization;

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
