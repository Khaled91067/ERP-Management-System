namespace ERP.Application.Features.Finance.Handlers;

using ERP.Application.Abstractions.Caching;
using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Finance.Commands;

using MediatR;

public sealed class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand, bool>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly ICacheService _cacheService;

    public async Task<bool> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return false;

        invoice.EnsureCanBeDeleted();

        _invoiceRepository.Delete(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"Finance:Invoice:{invoice.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync("Finance:Invoices", cancellationToken);

        return true;
    }
}


