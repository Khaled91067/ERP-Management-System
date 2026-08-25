
namespace ERP.Application.Features.Finance.Handlers;


using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Finance.Commands;
using ERP.Domain.Sales.Invoices;
using ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, bool>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public UpdateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id, cancellationToken);

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


