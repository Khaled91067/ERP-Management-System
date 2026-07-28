
namespace ERP.Application.Features.Catalog.Commands;

using MediatR;

public sealed record CreateProductCommand(
    string Name,
    string Sku,
    int CategoryId,
    decimal UnitPrice,
    decimal CostPrice,
    int InitialStockQuantity,
    int ReorderLevel) : IRequest<int>;