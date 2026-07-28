namespace ERP.Application.Features.Suppliers.Handlers;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Suppliers.DTOs;
using ERP.Application.Features.Suppliers.Queries;
using ERP.Domain.Purchasing.Suppliers;

using MediatR;

public sealed class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, PagedResult<SupplierDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSuppliersQueryHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<PagedResult<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Supplier>();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            options.Filter = s =>
                s.CompanyName.ToLower().Contains(term) ||
                s.ContactName.ToLower().Contains(term) ||
                s.Email.Value.ToLower().Contains(term);
        }

        var pagedSuppliers = await _supplierRepository.GetPagedAsync(options, request.Page, request.PageSize);

        return pagedSuppliers.Map(s => new SupplierDto(
            s.Id,
            s.CompanyName,
            s.ContactName,
            s.Email.Value,
            s.Phone,
            s.PaymentTerms));
    }
}
