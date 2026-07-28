
namespace ERP.Application.Features.Catalog.Commands;

using MediatR;

public sealed record DeleteCategoryCommand(int Id) : IRequest<bool>;