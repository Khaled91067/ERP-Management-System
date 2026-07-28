
namespace ERP.Application.Features.Catalog.Commands;

using MediatR;

public sealed record CreateCategoryCommand(string Name) : IRequest<int>;