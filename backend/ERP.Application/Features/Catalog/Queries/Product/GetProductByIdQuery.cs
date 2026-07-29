
namespace ERP.Application.Features.Catalog.Queries;

using global::ERP.Application.Features.Catalog.DTOs;

using MediatR;

public sealed record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
