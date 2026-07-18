using MediatR;

namespace ERP.Application.Features.Sales.Commands.Models;

public sealed record DeleteOrderCommand(int Id) : IRequest<bool>;
