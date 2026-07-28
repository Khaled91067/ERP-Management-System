
namespace ERP.Application.Features.Sales.Commands.Models;

using MediatR;

public sealed record DeleteCustomerCommand(
    int Id
) : IRequest<bool>;
