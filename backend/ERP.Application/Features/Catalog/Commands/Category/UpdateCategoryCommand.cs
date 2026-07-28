
namespace ERP.Application.Features.Catalog.Commands;

using MediatR;

public sealed record UpdateCategoryCommand(int Id, string Name) : IRequest<bool>;