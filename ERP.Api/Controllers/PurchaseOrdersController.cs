using ERP.Application.Features.PurchaseOrders.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

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
}