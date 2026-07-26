using MediatR;

namespace ERP.Application.Features.Catalog.Commands;

public sealed record DeleteProductCommand(int Id) : IRequest<bool>;