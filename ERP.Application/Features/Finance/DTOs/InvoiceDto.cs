using System;
using System.Collections.Generic;

namespace ERP.Application.Features.Finance.Dtos;

public sealed record InvoiceDto(
    int Id,
    int OrderId,
    int CustomerId,
    string CustomerName,
    DateTime InvoiceDate,
    DateTime DueDate,
    string Status,
    decimal TotalAmount,
    DateTime? PaidAt,
    List<InvoiceLineDto> Lines);

public sealed record InvoiceLineDto(
    int Id,
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal TaxRate,
    decimal TotalPrice);
