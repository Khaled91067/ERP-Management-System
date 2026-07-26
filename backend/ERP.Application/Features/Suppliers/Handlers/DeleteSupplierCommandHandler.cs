using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Suppliers.Commands;
using MediatR;

namespace ERP.Application.Features.Suppliers.Handlers;

public sealed class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, bool>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSupplierCommandHandler(
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id);

        if (supplier is null)
            return false;

        _supplierRepository.Delete(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
