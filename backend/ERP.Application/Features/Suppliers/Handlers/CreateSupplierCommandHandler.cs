using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Suppliers.Commands;
using ERP.Domain.Entities;
using MediatR;

namespace ERP.Application.Features.Suppliers.Handlers;

public sealed class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, int>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSupplierCommandHandler(
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = new Supplier
        {
            CompanyName = request.CompanyName.Trim(),
            ContactName = request.ContactName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            PaymentTerms = request.PaymentTerms.Trim()
        };

        _supplierRepository.Add(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}
