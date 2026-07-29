
namespace ERP.Application.Features.Sales.Commands;

using MediatR;

public sealed record CreateCustomerCommand(
    string Name,
    string Email,
    string Phone,
    string Address,
    string City,
    string Country,
    string TaxId
) : IRequest<int>;
