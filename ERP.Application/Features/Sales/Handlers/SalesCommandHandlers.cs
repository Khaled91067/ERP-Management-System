using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Commands.Models;
using ERP.Domain.Entities;
using ERP.Domain.Entities.Orders;
using ERP.Domain.Enums;
using ERP.Domain.Exceptions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.Sales.Handlers;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGenericRepository<Customer> _customerRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IGenericRepository<Customer> customerRepository,
        IGenericRepository<Product> productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer is null)
            throw new DomainException($"Customer with ID {request.CustomerId} was not found.");

        var order = new Order(
            request.CustomerId,
            request.PaymentMethod,
            request.ShippingAddress);

        foreach (var line in request.Lines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId);
            if (product is null)
                throw new DomainException($"Product with ID {line.ProductId} was not found.");

            // Deduct stock
            product.DecreaseStock(line.Quantity);
            _productRepository.Update(product);

            // Add line to order
            order.AddLine(
                line.ProductId,
                line.Quantity,
                line.UnitPrice,
                line.DiscountPercentage);
        }

        _orderRepository.Add(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}

public sealed class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
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
                throw new DomainException($"Unsupported status transition to {request.Status}");
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
