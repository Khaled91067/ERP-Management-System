namespace ERP.Application.Features.PurchaseOrders.Dtos;

using System;
using System.Collections.Generic;

public sealed record PurchaseOrderDto(
    int Id,
    int SupplierId,
    string SupplierName,
    DateTime OrderDate,
    DateTime ExpectedDelivery,
    string Status,
    decimal TotalAmount,
    List<PurchaseOrderLineDto>? Lines = null);

public sealed record PurchaseOrderLineDto(
    int Id,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitCost,
    decimal TotalCost);
