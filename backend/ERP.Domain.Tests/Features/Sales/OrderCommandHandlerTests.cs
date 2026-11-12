namespace ERP.Domain.Tests.Features.Sales;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Sales.Commands;
using ERP.Application.Features.Sales.Handlers;
using ERP.Domain.Catalog.Products;
using ERP.Domain.Sales.Orders;
using ERP.Domain.Shared.Exceptions;
using FluentAssertions;
using Xunit;

public class OrderCommandHandlerTests
{
    [Fact]
    public async Task UpdateOrderStatusCommandHandler_WhenCancelled_RestoresProductStockAndInvalidatesCache()
    {
        // Arrange
        var product1 = new Product("Laptop", "LAP-001", 1, 1000m, 700m, 5);
        product1.IncreaseStock(10);
        typeof(Product).GetProperty(nameof(Product.Id))!.SetValue(product1, 1);

        var product2 = new Product("Mouse", "MOU-001", 1, 50m, 20m, 10);
        product2.IncreaseStock(20);
        typeof(Product).GetProperty(nameof(Product.Id))!.SetValue(product2, 2);

        var order = new Order(1, PaymentMethod.CreditCard, "123 Main St");
        order.AddLine(productId: 1, quantity: 3, unitPrice: 1000m);
        order.AddLine(productId: 2, quantity: 5, unitPrice: 50m);
        product1.DecreaseStock(3); // 7 remaining
        product2.DecreaseStock(5); // 15 remaining

        var orderRepo = new FakeOrderRepository(order);
        var productRepo = new FakeProductRepository(product1, product2);
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new UpdateOrderStatusCommandHandler(orderRepo, productRepo, unitOfWork, cacheService);
        var command = new UpdateOrderStatusCommand(order.Id, OrderStatus.Cancelled);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
        product1.StockQuantity.Should().Be(10);
        product2.StockQuantity.Should().Be(20);
        unitOfWork.SaveCount.Should().Be(1);
        cacheService.RemovedPrefixes.Should().Contain("Sales:Orders");
        cacheService.RemovedPrefixes.Should().Contain("Catalog:Products");
        cacheService.RemovedKeys.Should().Contain("Catalog:Product:1");
        cacheService.RemovedKeys.Should().Contain("Catalog:Product:2");
    }

    [Fact]
    public async Task UpdateOrderStatusCommandHandler_WhenAlreadyCancelled_ThrowsAndDoesNotRestoreStockTwice()
    {
        // Arrange
        var product = new Product("Laptop", "LAP-001", 1, 1000m, 700m, 5);
        product.IncreaseStock(10);
        typeof(Product).GetProperty(nameof(Product.Id))!.SetValue(product, 1);

        var order = new Order(1, PaymentMethod.CreditCard, "123 Main St");
        order.AddLine(productId: 1, quantity: 3, unitPrice: 1000m);
        order.Cancel();

        var orderRepo = new FakeOrderRepository(order);
        var productRepo = new FakeProductRepository(product);
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new UpdateOrderStatusCommandHandler(orderRepo, productRepo, unitOfWork, cacheService);
        var command = new UpdateOrderStatusCommand(order.Id, OrderStatus.Cancelled);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleValidationException>().WithMessage("*already cancelled*");
        product.StockQuantity.Should().Be(10);
        unitOfWork.SaveCount.Should().Be(0);
    }

    [Fact]
    public async Task DeleteOrderCommandHandler_WithPendingOrder_RestoresStockAndDeletesOrder()
    {
        // Arrange
        var product = new Product("Monitor", "MON-001", 1, 300m, 200m, 5);
        product.IncreaseStock(10);
        typeof(Product).GetProperty(nameof(Product.Id))!.SetValue(product, 1);

        var order = new Order(1, PaymentMethod.Cash, "456 Oak Ave");
        order.AddLine(productId: 1, quantity: 4, unitPrice: 300m);
        product.DecreaseStock(4); // 6 remaining

        var orderRepo = new FakeOrderRepository(order);
        var productRepo = new FakeProductRepository(product);
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new DeleteOrderCommandHandler(orderRepo, productRepo, unitOfWork, cacheService);
        var command = new DeleteOrderCommand(order.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        product.StockQuantity.Should().Be(10);
        orderRepo.DeletedOrders.Should().Contain(order);
        unitOfWork.SaveCount.Should().Be(1);
        cacheService.RemovedPrefixes.Should().Contain("Sales:Orders");
        cacheService.RemovedPrefixes.Should().Contain("Catalog:Products");
        cacheService.RemovedKeys.Should().Contain("Catalog:Product:1");
    }

    private sealed class FakeOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders = new();
        public List<Order> DeletedOrders { get; } = new();

        public FakeOrderRepository(params Order[] orders)
        {
            _orders.AddRange(orders);
            for (var i = 0; i < _orders.Count; i++)
            {
                if (_orders[i].Id == 0)
                {
                    typeof(Order).GetProperty(nameof(Order.Id))!.SetValue(_orders[i], i + 1);
                }
            }
        }

        public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(_orders.FirstOrDefault(x => x.Id == id));

        public Task<Order?> GetByIdWithLinesAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(_orders.FirstOrDefault(x => x.Id == id));

        public Task<IEnumerable<Order>> GetAllAsync(QueryOptions<Order>? options = null, CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<Order>>(_orders);

        public Task<PagedResult<Order>> GetPagedAsync(QueryOptions<Order>? options = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<Order>(_orders, _orders.Count, page, pageSize));

        public void Add(Order entity) => _orders.Add(entity);
        public void Update(Order entity) { }
        public void Delete(Order entity)
        {
            DeletedOrders.Add(entity);
            _orders.Remove(entity);
        }
    }

    private sealed class FakeProductRepository : IGenericRepository<Product>
    {
        private readonly List<Product> _products = new();

        public FakeProductRepository(params Product[] products)
        {
            _products.AddRange(products);
        }

        public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(_products.FirstOrDefault(x => x.Id == id));

        public Task<IEnumerable<Product>> GetAllAsync(QueryOptions<Product>? options = null, CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<Product>>(_products);

        public Task<PagedResult<Product>> GetPagedAsync(QueryOptions<Product>? options = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<Product>(_products, _products.Count, page, pageSize));

        public void Add(Product entity) => _products.Add(entity);
        public void Update(Product entity) { }
        public void Delete(Product entity) => _products.Remove(entity);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveCount++;
            return Task.FromResult(1);
        }
    }

    private sealed class FakeCacheService : ICacheService
    {
        public List<string> RemovedKeys { get; } = new();
        public List<string> RemovedPrefixes { get; } = new();

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) => Task.FromResult(default(T));
        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            RemovedKeys.Add(key);
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            RemovedPrefixes.Add(prefix);
            return Task.CompletedTask;
        }

        public Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null, bool cacheNullValue = false, CancellationToken cancellationToken = default)
            => factory(cancellationToken);
    }
}
