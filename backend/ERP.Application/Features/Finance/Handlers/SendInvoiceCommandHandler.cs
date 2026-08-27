namespace ERP.Application.Features.Finance.Handlers;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Finance.Commands;
using MediatR;

public sealed class SendInvoiceCommandHandler : IRequestHandler<SendInvoiceCommand, bool>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public SendInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(SendInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithLinesAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return false;

        invoice.Send();

        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"Finance:Invoice:{invoice.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync("Finance:Invoices", cancellationToken);

        return true;
    }
}
