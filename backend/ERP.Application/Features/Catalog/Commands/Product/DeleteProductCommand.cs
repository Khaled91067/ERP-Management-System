
namespace ERP.Application.Features.Catalog.Commands;

using MediatR;

public sealed record DeleteProductCommand(int Id) : IRequest<bool>;