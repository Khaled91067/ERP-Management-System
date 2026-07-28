
namespace ERP.Application.Features.Sales.Commands.Models;

using MediatR;

public sealed record DeleteOrderCommand(int Id) : IRequest<bool>;
