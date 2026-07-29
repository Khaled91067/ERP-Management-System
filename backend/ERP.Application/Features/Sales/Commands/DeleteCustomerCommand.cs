
namespace ERP.Application.Features.Sales.Commands;

using MediatR;

public sealed record DeleteCustomerCommand(
    int Id
) : IRequest<bool>;
