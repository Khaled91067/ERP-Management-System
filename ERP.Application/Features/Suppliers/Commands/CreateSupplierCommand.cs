using MediatR;

namespace ERP.Application.Features.Suppliers.Commands;

public sealed record CreateSupplierCommand(
    string CompanyName,
    string ContactName,
    string Email,
    string Phone,
    string PaymentTerms) : IRequest<int>;
