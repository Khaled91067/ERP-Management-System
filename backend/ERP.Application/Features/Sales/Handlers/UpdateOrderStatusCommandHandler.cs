namespace ERP.Application.Features.Sales.Handlers;

using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Sales.Commands;
using global::ERP.Domain.Sales.Orders;
using global::ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithLinesAsync(request.OrderId, cancellationToken);
        if (order is null)
            return false;

        switch (request.Status)
        {
            case OrderStatus.Shipped:
                order.Ship();
                break;
            case OrderStatus.Delivered:
                order.Deliver();
                break;
            case OrderStatus.Cancelled:
                order.Cancel();
                break;
            default:
                throw new BusinessRuleValidationException($"Unsupported status transition to {request.Status}");
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(global::ERP.Application.Common.Caching.CacheKeys.Sales.OrderById(order.Id), cancellationToken);
        await _cacheService.RemoveByPrefixAsync(global::ERP.Application.Common.Caching.CacheKeys.Sales.OrdersPrefix(), cancellationToken);

        return true;
    }
}


