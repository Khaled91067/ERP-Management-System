using MediatR;

namespace ERP.Application.Features.Catalog.Commands;

public sealed record DeleteCategoryCommand(int Id) : IRequest<bool>;