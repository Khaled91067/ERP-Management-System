using ERP.Application.Features.Identity.Commands;
using ERP.Application.Features.Identity.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/admin/users")]
public sealed class AdminUsersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetUsersQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByIdQuery(id), cancellationToken);
        
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var updated = await sender.Send(new UpdateUserCommand(id, request.FirstName, request.LastName, request.Email), cancellationToken);
        
        return updated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/role")]
    public async Task<IActionResult> ChangeRole(int id, ChangeUserRoleRequest request, CancellationToken cancellationToken)
    {
        var updated = await sender.Send(new ChangeUserRoleCommand(id, request.RoleId), cancellationToken);
        
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var updated = await sender.Send(new DeleteUserCommand(id), cancellationToken);
       
        return updated ? NoContent() : NotFound();
    }
}

public sealed record UpdateUserRequest(string FirstName, string LastName, string Email);
public sealed record ChangeUserRoleRequest(int RoleId);
