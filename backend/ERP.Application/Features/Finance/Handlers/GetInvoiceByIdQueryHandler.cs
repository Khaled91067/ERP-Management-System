
namespace ERP.Application.Features.Finance.Handlers;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Finance.Dtos;
using global::ERP.Application.Features.Finance.Queries.Models;

using MediatR;

using global::ERP.Application.Abstractions.Caching;
using global::ERP.Application.Common.Caching;
using global::Microsoft.Extensions.Options;

public sealed class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto?>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICacheService _cacheService;
    private readonly IOptions<CacheSettings> _cacheSettings;

    public GetInvoiceByIdQueryHandler(
        IInvoiceRepository invoiceRepository,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings)
    {
        _invoiceRepository = invoiceRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"Finance:Invoice:{request.Id}";
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.FrequentDataExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var invoice = await _invoiceRepository.GetByIdWithLinesAsync(request.Id, cancellationToken);
                if (invoice is null)
                    return null;

                var linesDto = invoice.InvoiceLines.Select(il => new InvoiceLineDto(
                    il.Id,
                    il.Description,
                    il.Quantity,
                    il.UnitPrice.Amount,
                    il.TaxRate,
                    (il.Quantity * il.UnitPrice.Amount) * (1 + il.TaxRate / 100)
                )).ToList();

                return new InvoiceDto(
                    invoice.Id,
                    invoice.OrderId,
                    invoice.CustomerId,
                    invoice.Customer?.Name ?? string.Empty,
                    invoice.InvoiceDate,
                    invoice.DueDate,
                    invoice.Status.ToString(),
                    invoice.TotalAmount.Amount,
                    invoice.PaidAt,
                    linesDto);
            },
            expiration,
            true,
            cancellationToken);
    }
}

