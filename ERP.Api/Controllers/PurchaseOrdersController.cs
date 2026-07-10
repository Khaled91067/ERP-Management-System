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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id,CancellationToken cancellationToken)
    {
        var query = new GetPurchaseOrderByIdQuery(id);

        var result = await _sender.Send(query,cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePurchaseOrderCommand command,CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            new { id });
    }

}