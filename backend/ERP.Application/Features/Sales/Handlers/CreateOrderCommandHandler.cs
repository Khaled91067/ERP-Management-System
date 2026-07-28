
namespace ERP.Application.Features.Sales.Handlers;

using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Commands.Models;
using ERP.Domain.Catalog.Products;
using ERP.Domain.Sales.Customers;
using ERP.Domain.Sales.Orders;
using ERP.Domain.Shared.Exceptions;

using MediatR;

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
            throw new NotFoundException("Customer", request.CustomerId);

        var order = new Order(
            request.CustomerId,
            request.PaymentMethod,
            request.ShippingAddress);

        foreach (var line in request.Lines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId);
            if (product is null)
                throw new NotFoundException("Product", line.ProductId);

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
