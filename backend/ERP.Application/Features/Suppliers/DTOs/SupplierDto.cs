namespace ERP.Application.Features.Suppliers.DTOs;

public sealed record SupplierDto(
    int Id,
    string CompanyName,
    string ContactName,
    string Email,
    string Phone,
    string PaymentTerms);
