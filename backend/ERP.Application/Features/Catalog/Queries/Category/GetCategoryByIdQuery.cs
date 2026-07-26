using ERP.Application.Features.Catalog.DTOs;
using MediatR;

namespace ERP.Application.Features.Catalog.Queries;

public sealed record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto?>;