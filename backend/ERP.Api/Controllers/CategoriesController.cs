namespace ERP.Api.Controllers;

using ERP.Application.Features.Catalog.Commands;
using ERP.Application.Features.Catalog.Queries;
using ERP.Domain.Identity.Permissions;
using ERP.Infrastructure.Authentication.Permissions;

using MediatR;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.Categories.Read)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default) =>
        Ok(await sender.Send(new GetCategoriesQuery(search, page, pageSize), cancellationToken));

    [HttpGet("{id:int}")]
    [HasPermission(Permissions.Categories.Read)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var category = await sender.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    [HasPermission(Permissions.Categories.Write)]
    public async Task<IActionResult> Create(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [HasPermission(Permissions.Categories.Write)]
    public async Task<IActionResult> Update(int id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var updated = await sender.Send(new UpdateCategoryCommand(id, request.Name), cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [HasPermission(Permissions.Categories.Write)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await sender.Send(new DeleteCategoryCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

public sealed record UpdateCategoryRequest(string Name);
