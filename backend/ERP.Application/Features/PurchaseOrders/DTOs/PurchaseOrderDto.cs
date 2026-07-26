namespace ERP.Application.Features.PurchaseOrders.Dtos;

public sealed record PurchaseOrderDto(
    int Id,
    int SupplierId,
    DateTime OrderDate,
    DateTime ExpectedDelivery,
    string Status,
    decimal TotalAmount);