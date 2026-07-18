using ERP.Application.Features.Suppliers.DTOs;
using MediatR;

namespace ERP.Application.Features.Suppliers.Queries;

public sealed record GetSuppliersQuery(string? SearchTerm) : IRequest<IEnumerable<SupplierDto>>;
