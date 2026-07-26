using MediatR;

namespace ERP.Application.Features.Sales.Commands.Models;

public sealed record DeleteCustomerCommand(
    int Id
) : IRequest<bool>;
