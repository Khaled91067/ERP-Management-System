
namespace ERP.Application.Features.Catalog.Queries;

using global::ERP.Application.Features.Catalog.DTOs;

using MediatR;

public sealed record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto?>;
