namespace ERP.Api.Controllers;

using ERP.Application.Features.PurchaseOrders.Commands;
using ERP.Application.Features.PurchaseOrders.Queries;
using ERP.Domain.Identity.Permissions;
using ERP.Infrastructure.Authentication.Permissions;

using MediatR;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/purchase-orders")]
public sealed class PurchaseOrdersController : ControllerBase
{
    private readonly ISender _sender;

    public PurchaseOrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(Permissions.PurchaseOrders.Read)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? supplierId,
        [FromQuery] string? status,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPurchaseOrdersQuery(supplierId, status, search, page, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [HasPermission(Permissions.PurchaseOrders.Read)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetPurchaseOrderByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.PurchaseOrders.Write)]
    public async Task<IActionResult> Create(CreatePurchaseOrderCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            new { id });
    }

    [HttpPatch("{id:int}/status")]
    [HasPermission(Permissions.PurchaseOrders.Write)]
    public async Task<IActionResult> UpdateStatus(int id, UpdatePurchaseOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdatePurchaseOrderStatusCommand(id, request.Status);
        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [HasPermission(Permissions.PurchaseOrders.Write)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeletePurchaseOrderCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }
}

public sealed record UpdatePurchaseOrderStatusRequest(string Status);
