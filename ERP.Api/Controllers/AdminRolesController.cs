using ERP.Application.Features.Identity.Commands;
using ERP.Application.Features.Identity.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/admin/roles")]
public sealed class AdminRolesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRolesQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var role = await sender.Send(new GetRoleByIdQuery(id), cancellationToken);
        return role is null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var updated = await sender.Send(new UpdateRoleCommand(id, request.Name, request.Permissions), cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await sender.Send(new DeleteRoleCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

public sealed record UpdateRoleRequest(string Name, string Permissions);
