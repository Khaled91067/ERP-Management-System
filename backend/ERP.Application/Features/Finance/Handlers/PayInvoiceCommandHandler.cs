namespace ERP.Application.Features.Finance.Handlers;

using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Finance.Commands.Models;
using global::ERP.Domain.Sales.Invoices;

using MediatR;

public sealed class PayInvoiceCommandHandler : IRequestHandler<PayInvoiceCommand, bool>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;

    public PayInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithLinesAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return false;

        // If the invoice is still a Draft, transition it to Sent first so it can be paid
        if (invoice.Status == InvoiceStatus.Draft)
        {
            invoice.Send();
        }

        invoice.Pay();

        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"Finance:Invoice:{invoice.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync("Finance:Invoices", cancellationToken);

        return true;
    }
}


