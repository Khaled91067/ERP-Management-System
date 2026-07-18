using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Commands.Models;
using ERP.Domain.Enums;
using ERP.Domain.Exceptions;
using MediatR;

namespace ERP.Application.Features.Sales.Handlers;

public sealed class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id);

        if (order is null)
            return false;

        if (order.Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be deleted.");

        order.Cancel();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
