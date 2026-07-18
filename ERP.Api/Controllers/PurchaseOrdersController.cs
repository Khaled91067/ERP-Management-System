using ERP.Application.Features.PurchaseOrders.Commands.Models;
using ERP.Application.Features.PurchaseOrders.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[Authorize]
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
    public async Task<IActionResult> GetAll(
        [FromQuery] int? supplierId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var query = new GetPurchaseOrdersQuery(supplierId, status);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetPurchaseOrderByIdQuery(id);

        var result = await _sender.Send(query, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePurchaseOrderCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            new { id });
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdatePurchaseOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdatePurchaseOrderStatusCommand(id, request.Status);
        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }
}

public sealed record UpdatePurchaseOrderStatusRequest(string Status);