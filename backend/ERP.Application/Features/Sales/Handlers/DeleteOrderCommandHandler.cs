namespace ERP.Application.Features.Sales.Handlers;

using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Commands;

using MediatR;

public sealed class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly ICacheService _cacheService;

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
            return false;

        order.EnsureCanBeDeleted();

        order.Cancel();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"Sales:Order:{order.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync("Sales:Orders", cancellationToken);

        return true;
    }
}


