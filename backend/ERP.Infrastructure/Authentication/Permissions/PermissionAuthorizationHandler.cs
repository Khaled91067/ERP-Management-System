namespace ERP.Infrastructure.Authentication.Permissions;

using System.Security.Claims;
using System.Threading.Tasks;
using ERP.Domain.Identity.Roles;
using Microsoft.AspNetCore.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        // Administrator role automatically satisfies all permissions
        if (context.User.IsInRole(RoleConstants.Administrator) ||
            context.User.HasClaim(ClaimTypes.Role, RoleConstants.Administrator))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Check explicit permission claim
        if (context.User.HasClaim(c => c.Type == "permission" && c.Value == requirement.Permission))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}
