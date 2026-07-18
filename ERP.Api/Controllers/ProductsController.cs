using ERP.Application.Features.Catalog.Commands;
using ERP.Application.Features.Catalog.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/products")]
public sealed class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetProductsQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await sender.Send(new GetProductByIdQuery(id), cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var updated = await sender.Send(new UpdateProductCommand(id, request.Name, request.Sku, request.CategoryId,
            request.UnitPrice, request.CostPrice, request.ReorderLevel), cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await sender.Send(new DeleteProductCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/stock")]
    public async Task<IActionResult> AdjustStock(int id, [FromBody] AdjustStockRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AdjustStockCommand(id, request.QuantityChange), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}

public sealed record UpdateProductRequest(
    string Name,
    string Sku,
    int CategoryId,
    decimal UnitPrice,
    decimal CostPrice,
    int ReorderLevel);

public sealed record AdjustStockRequest(int QuantityChange);
