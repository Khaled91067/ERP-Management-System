using MediatR;

namespace ERP.Application.Features.Catalog.Commands;

public sealed record UpdateProductCommand(
    int Id,
    string Name,
    string Sku,
    int CategoryId,
    decimal UnitPrice,
    decimal CostPrice,
    int ReorderLevel) : IRequest<bool>;