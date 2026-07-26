namespace ERP.Application.Features.Catalog.DTOs;

public sealed record ProductDto(
    int Id,
    string Name,
    string Sku,
    int CategoryId,
    string CategoryName,
    decimal UnitPrice,
    decimal CostPrice,
    int StockQuantity,
    int ReorderLevel);
