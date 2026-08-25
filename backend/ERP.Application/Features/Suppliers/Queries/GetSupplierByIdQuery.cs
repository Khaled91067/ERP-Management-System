
namespace ERP.Application.Features.Suppliers.Queries;

using ERP.Application.Features.Suppliers.DTOs;

using MediatR;

public sealed record GetSupplierByIdQuery(int Id) : IRequest<SupplierDto?>;

