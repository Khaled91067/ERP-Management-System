using ERP.Application.Features.HR.Commands.Models;
using ERP.Application.Features.HR.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/employees")]
public sealed class EmployeesController : ControllerBase
{
    private readonly ISender _sender;

    public EmployeesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? departmentId,
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken)
    {
        var query = new GetEmployeesQuery(departmentId, searchTerm);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetEmployeeByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployeeCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEmployeeCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Employee ID mismatch between route and request body.");

        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteEmployeeCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
