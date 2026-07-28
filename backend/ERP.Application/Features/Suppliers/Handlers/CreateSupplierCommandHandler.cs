namespace ERP.Application.Features.Suppliers.Handlers;

using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Suppliers.Commands;
using ERP.Domain.Purchasing.Suppliers;

using MediatR;

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
        var supplier = new Supplier(
            request.CompanyName,
            request.Email,
            request.ContactName,
            request.Phone,
            request.PaymentTerms);

        _supplierRepository.Add(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}
