namespace ERP.Application.Features.Sales.Dtos;

public sealed record CustomerDto(
    int Id,
    string Name,
    string Email,
    string Phone,
    string Address,
    string City,
    string Country,
    string TaxId);
