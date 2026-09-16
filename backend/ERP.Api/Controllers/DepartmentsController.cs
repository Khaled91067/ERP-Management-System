namespace ERP.Api.Controllers;

using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Features.HR.Commands;
using ERP.Application.Features.HR.Queries;
using ERP.Domain.Identity.Permissions;
using ERP.Infrastructure.Authentication.Permissions;

using MediatR;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController : ControllerBase
{
    private readonly ISender _sender;

    public DepartmentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(Permissions.Departments.Read)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDepartmentsQuery(search, page, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Departments.Write)]
    public async Task<IActionResult> Create(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return Ok(new { id });
    }

    [HttpDelete("{id:int}")]
    [HasPermission(Permissions.Departments.Write)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteDepartmentCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
