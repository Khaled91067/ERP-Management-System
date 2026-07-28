
namespace ERP.Application.Features.Suppliers.Commands;

using MediatR;

public sealed record UpdateSupplierCommand(
    int Id,
    string CompanyName,
    string ContactName,
    string Email,
    string Phone,
    string PaymentTerms) : IRequest<bool>;
