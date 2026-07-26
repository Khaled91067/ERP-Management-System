using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Suppliers.DTOs;
using ERP.Application.Features.Suppliers.Queries;
using MediatR;

namespace ERP.Application.Features.Suppliers.Handlers;

public sealed class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDto?>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSupplierByIdQueryHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierDto?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id);

        if (supplier is null)
            return null;

        return new SupplierDto(
            supplier.Id,
            supplier.CompanyName,
            supplier.ContactName,
            supplier.Email,
            supplier.Phone,
            supplier.PaymentTerms);
    }
}
