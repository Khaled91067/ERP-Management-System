
namespace ERP.Application.Features.Sales.Commands;

using MediatR;

public sealed record DeleteOrderCommand(int Id) : IRequest<bool>;
