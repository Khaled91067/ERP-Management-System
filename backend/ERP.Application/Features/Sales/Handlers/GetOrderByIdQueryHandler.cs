
namespace ERP.Application.Features.Sales.Handlers;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Sales.Dtos;
using global::ERP.Application.Features.Sales.Queries;

using MediatR;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;
    private readonly global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> _cacheSettings;

    public GetOrderByIdQueryHandler(
        IOrderRepository orderRepository,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService,
        global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> cacheSettings)
    {
        _orderRepository = orderRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"Sales:Order:{request.Id}";
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.FrequentDataExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var order = await _orderRepository.GetByIdWithLinesAsync(request.Id, cancellationToken);

                if (order is null)
                    return null;

                var linesDto = order.OrderLines.Select(ol => new OrderLineDto(
                    ol.Id,
                    ol.ProductId,
                    ol.Product?.Name ?? string.Empty,
                    ol.Quantity,
                    ol.UnitPrice.Amount,
                    ol.DiscountPercentage,
                    (ol.Quantity * ol.UnitPrice.Amount) * (1 - ol.DiscountPercentage / 100)
                )).ToList();

                return new OrderDto(
                    order.Id,
                    order.CustomerId,
                    order.Customer?.Name ?? string.Empty,
                    order.OrderDate,
                    order.Status.ToString(),
                    order.PaymentMethod.ToString(),
                    order.ShippingAddress,
                    order.TotalAmount.Amount,
                    linesDto);
            },
            expiration,
            true,
            cancellationToken);
    }
}


