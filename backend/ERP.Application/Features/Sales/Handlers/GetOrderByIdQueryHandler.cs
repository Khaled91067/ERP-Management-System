
namespace ERP.Application.Features.Sales.Handlers;

using ERP.Application.Common.Caching;
using Microsoft.Extensions.Options;
using ERP.Application.Abstractions.Caching;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Dtos;
using ERP.Application.Features.Sales.Queries;

using MediatR;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICacheService _cacheService;
    private readonly IOptions<CacheSettings> _cacheSettings;

    public GetOrderByIdQueryHandler(
        IOrderRepository orderRepository,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings)
    {
        _orderRepository = orderRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Sales.OrderById(request.Id);
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


