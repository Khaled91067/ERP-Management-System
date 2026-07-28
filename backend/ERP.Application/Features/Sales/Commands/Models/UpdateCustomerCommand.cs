
namespace ERP.Application.Features.Sales.Commands.Models;

using MediatR;

public sealed record UpdateCustomerCommand(
    int Id,
    string Name,
    string Email,
    string Phone,
    string Address,
    string City,
    string Country,
    string TaxId
) : IRequest<bool>;
