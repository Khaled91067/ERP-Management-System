using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Suppliers.DTOs;
using ERP.Application.Features.Suppliers.Queries;
using MediatR;

namespace ERP.Application.Features.Suppliers.Handlers;

public sealed class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, IEnumerable<SupplierDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSuppliersQueryHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<IEnumerable<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Domain.Entities.Supplier>();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            options.Filter = s =>
                s.CompanyName.ToLower().Contains(term) ||
                s.ContactName.ToLower().Contains(term) ||
                s.Email.ToLower().Contains(term);
        }

        var suppliers = await _supplierRepository.GetAllAsync(options);

        return suppliers.Select(s => new SupplierDto(
            s.Id,
            s.CompanyName,
            s.ContactName,
            s.Email,
            s.Phone,
            s.PaymentTerms));
    }
}
