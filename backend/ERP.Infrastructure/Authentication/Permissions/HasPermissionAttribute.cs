namespace ERP.Infrastructure.Authentication.Permissions;

using Microsoft.AspNetCore.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(policy: permission)
    {
        Permission = permission;
    }

    public string Permission { get; }
}
