using MediatR;

namespace ERP.Application.Features.Catalog.Commands;

public sealed record UpdateCategoryCommand(int Id, string Name) : IRequest<bool>;