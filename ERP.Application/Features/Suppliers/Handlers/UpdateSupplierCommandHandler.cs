using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Suppliers.Commands;
using MediatR;

namespace ERP.Application.Features.Suppliers.Handlers;

public sealed class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, bool>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSupplierCommandHandler(
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id);

        if (supplier is null)
            return false;

        supplier.CompanyName = request.CompanyName.Trim();
        supplier.ContactName = request.ContactName.Trim();
        supplier.Email = request.Email.Trim();
        supplier.Phone = request.Phone.Trim();
        supplier.PaymentTerms = request.PaymentTerms.Trim();

        _supplierRepository.Update(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
