using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Finance.Commands.Models;
using ERP.Domain.Enums;
using ERP.Domain.Exceptions;
using MediatR;

namespace ERP.Application.Features.Finance.Handlers;

public sealed class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, bool>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id);

        if (invoice is null)
            return false;

        if (invoice.Status != InvoiceStatus.Draft)
            throw new DomainException("Only draft invoices can be updated.");

        // DueDate is private set in domain, so we track the entity as modified
        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
