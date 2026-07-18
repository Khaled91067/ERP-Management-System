using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Commands.Models;
using ERP.Domain.Entities;
using ERP.Domain.Exceptions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.Sales.Handlers;

public sealed class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderCommandHandler(
        IOrderRepository orderRepository,
        IGenericRepository<Product> productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithLinesAsync(request.Id, cancellationToken);
        if (order is null)
            return false;

        // 1. Revert stock of existing lines
        foreach (var existingLine in order.OrderLines)
        {
            var product = await _productRepository.GetByIdAsync(existingLine.ProductId);
            if (product != null)
            {
                product.IncreaseStock(existingLine.Quantity);
                _productRepository.Update(product);
            }
        }

        // 2. Clear old lines
        order.ClearLines();

        // 3. Update top-level details
        order.UpdateDetails(request.PaymentMethod, request.ShippingAddress);

        // 4. Add new lines and deduct stock
        foreach (var line in request.Lines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId);
            if (product is null)
                throw new DomainException($"Product with ID {line.ProductId} was not found.");

            product.DecreaseStock(line.Quantity);
            _productRepository.Update(product);

            order.AddLine(
                line.ProductId,
                line.Quantity,
                line.UnitPrice,
                line.DiscountPercentage);
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
