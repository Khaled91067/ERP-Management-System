using MediatR;

namespace ERP.Application.Features.Catalog.Commands;

public sealed record CreateCategoryCommand(string Name) : IRequest<int>;