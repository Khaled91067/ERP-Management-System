
namespace ERP.Application.Features.Finance.Handlers;


using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Finance.Commands;
using global::ERP.Domain.Sales.Invoices;
using global::ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, bool>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;

    public UpdateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id);

        if (invoice is null)
            return false;

        if (invoice.Status != InvoiceStatus.Draft)
            throw new BusinessRuleValidationException("Only draft invoices can be updated.");

        // DueDate is private set in domain, so we track the entity as modified
        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"Finance:Invoice:{invoice.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync("Finance:Invoices", cancellationToken);

        return true;
    }
}


