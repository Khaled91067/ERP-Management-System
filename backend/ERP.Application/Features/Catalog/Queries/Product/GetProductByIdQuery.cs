using ERP.Application.Features.Catalog.DTOs;
using MediatR;

namespace ERP.Application.Features.Catalog.Queries;

public sealed record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;