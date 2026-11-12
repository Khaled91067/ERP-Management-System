namespace ERP.Domain.Tests.Features.Finance;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Finance.Commands;
using ERP.Application.Features.Finance.Handlers;
using ERP.Domain.Catalog.Products;
using ERP.Domain.Sales.Customers;
using ERP.Domain.Sales.Invoices;
using ERP.Domain.Sales.Orders;
using ERP.Domain.Shared.Exceptions;
using FluentAssertions;
using Xunit;

public class FinanceCommandHandlerTests
{
    private static DateTime FutureDueDate => DateTime.UtcNow.Date.AddDays(14);

    [Fact]
    public async Task CreateInvoiceCommandHandler_WithStandaloneInvoice_CreatesInvoiceWithoutOrderId()
    {
        // Arrange
        var customer = new Customer("ACME Corp", "billing@acme.com");
        var invoiceRepo = new FakeInvoiceRepository();
        var customerRepo = new FakeCustomerRepository(customer);
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new CreateInvoiceCommandHandler(invoiceRepo, customerRepo, unitOfWork, cacheService);
        var command = new CreateInvoiceCommand(
            CustomerId: customer.Id,
            DueDate: FutureDueDate,
            Lines: new List<CreateInvoiceLineCommand>
            {
                new("Consulting Services", 10, 150m, 5m)
            },
            OrderId: null);

        // Act
        var resultId = await handler.Handle(command, CancellationToken.None);

        // Assert
        invoiceRepo.Invoices.Should().HaveCount(1);
        var created = invoiceRepo.Invoices.First();
        created.OrderId.Should().BeNull();
        created.CustomerId.Should().Be(customer.Id);
        created.Status.Should().Be(InvoiceStatus.Draft);
        created.TotalAmount.Amount.Should().Be(10 * 150m * 1.05m);
        unitOfWork.SaveCount.Should().Be(1);
        cacheService.RemovedPrefixes.Should().Contain("Finance:Invoices");
    }

    [Fact]
    public async Task CreateInvoiceCommandHandler_WithOrderId_CreatesInvoiceLinkedToOrder()
    {
        // Arrange
        var customer = new Customer("ACME Corp", "billing@acme.com");
        var invoiceRepo = new FakeInvoiceRepository();
        var customerRepo = new FakeCustomerRepository(customer);
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new CreateInvoiceCommandHandler(invoiceRepo, customerRepo, unitOfWork, cacheService);
        var command = new CreateInvoiceCommand(
            CustomerId: customer.Id,
            DueDate: FutureDueDate,
            Lines: new List<CreateInvoiceLineCommand>
            {
                new("Hardware Setup", 1, 500m, 0m)
            },
            OrderId: 42);

        // Act
        var resultId = await handler.Handle(command, CancellationToken.None);

        // Assert
        var created = invoiceRepo.Invoices.First();
        created.OrderId.Should().Be(42);
        created.CustomerId.Should().Be(customer.Id);
    }

    [Fact]
    public async Task CreateInvoiceCommandHandler_WithNonExistentCustomer_ThrowsNotFoundException()
    {
        // Arrange
        var invoiceRepo = new FakeInvoiceRepository();
        var customerRepo = new FakeCustomerRepository();
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new CreateInvoiceCommandHandler(invoiceRepo, customerRepo, unitOfWork, cacheService);
        var command = new CreateInvoiceCommand(
            CustomerId: 999,
            DueDate: FutureDueDate,
            Lines: new List<CreateInvoiceLineCommand> { new("Item", 1, 10m) });

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateInvoiceCommandHandler_WithNonExistentOrderId_ThrowsNotFoundException()
    {
        // Arrange
        var customer = new Customer("ACME Corp", "billing@acme.com");
        var invoiceRepo = new FakeInvoiceRepository();
        var customerRepo = new FakeCustomerRepository(customer);
        var orderRepo = new FakeOrderRepository();
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new CreateInvoiceCommandHandler(invoiceRepo, customerRepo, unitOfWork, cacheService, orderRepo);
        var command = new CreateInvoiceCommand(
            CustomerId: customer.Id,
            DueDate: FutureDueDate,
            Lines: new List<CreateInvoiceLineCommand> { new("Item", 1, 10m) },
            OrderId: 999);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GenerateInvoiceCommandHandler_WithValidOrder_GeneratesInvoiceAndInvalidatesCache()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");
        order.AddLine(productId: 10, quantity: 2, unitPrice: 100m, discountPercentage: 10m);

        var invoiceRepo = new FakeInvoiceRepository();
        var orderRepo = new FakeOrderRepository(order);
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new GenerateInvoiceCommandHandler(invoiceRepo, orderRepo, unitOfWork, cacheService);
        var command = new GenerateInvoiceCommand(order.Id, FutureDueDate);

        // Act
        var resultId = await handler.Handle(command, CancellationToken.None);

        // Assert
        invoiceRepo.Invoices.Should().HaveCount(1);
        var created = invoiceRepo.Invoices.First();
        created.OrderId.Should().Be(order.Id);
        created.CustomerId.Should().Be(order.CustomerId);
        created.InvoiceLines.Should().HaveCount(1);
        // (2 * 90) = 180
        created.TotalAmount.Amount.Should().Be(180m);
        unitOfWork.SaveCount.Should().Be(1);
        cacheService.RemovedPrefixes.Should().Contain("Finance:Invoices");
    }

    [Fact]
    public async Task GenerateInvoiceCommandHandler_WithCancelledOrder_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");
        order.Cancel();

        var invoiceRepo = new FakeInvoiceRepository();
        var orderRepo = new FakeOrderRepository(order);
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new GenerateInvoiceCommandHandler(invoiceRepo, orderRepo, unitOfWork, cacheService);
        var command = new GenerateInvoiceCommand(order.Id, FutureDueDate);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleValidationException>().WithMessage("*cancelled order*");
    }

    [Fact]
    public async Task SendInvoiceCommandHandler_WithDraftInvoice_TransitionsToSentAndInvalidatesCache()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);
        invoice.AddLine("Item", 1, 100m);

        var invoiceRepo = new FakeInvoiceRepository(invoice);
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new SendInvoiceCommandHandler(invoiceRepo, unitOfWork, cacheService);
        var command = new SendInvoiceCommand(invoice.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        invoice.Status.Should().Be(InvoiceStatus.Sent);
        unitOfWork.SaveCount.Should().Be(1);
        cacheService.RemovedKeys.Should().Contain($"Finance:Invoice:{invoice.Id}");
        cacheService.RemovedPrefixes.Should().Contain("Finance:Invoices");
    }

    [Fact]
    public async Task SendInvoiceCommandHandler_WithNonExistentInvoice_ReturnsFalse()
    {
        // Arrange
        var invoiceRepo = new FakeInvoiceRepository();
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new SendInvoiceCommandHandler(invoiceRepo, unitOfWork, cacheService);
        var command = new SendInvoiceCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task PayInvoiceCommandHandler_WithDraftInvoice_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);
        invoice.AddLine("Item", 1, 100m);

        var invoiceRepo = new FakeInvoiceRepository(invoice);
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new PayInvoiceCommandHandler(invoiceRepo, unitOfWork, cacheService);
        var command = new PayInvoiceCommand(invoice.Id);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleValidationException>().WithMessage("*Draft invoices must be sent before they can be marked as paid*");
    }

    [Fact]
    public async Task PayInvoiceCommandHandler_WithSentInvoice_PaysInvoiceAndInvalidatesCache()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);
        invoice.AddLine("Item", 1, 100m);
        invoice.Send();

        var invoiceRepo = new FakeInvoiceRepository(invoice);
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new PayInvoiceCommandHandler(invoiceRepo, unitOfWork, cacheService);
        var command = new PayInvoiceCommand(invoice.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        invoice.Status.Should().Be(InvoiceStatus.Paid);
        invoice.PaidAt.Should().NotBeNull();
        unitOfWork.SaveCount.Should().Be(1);
        cacheService.RemovedKeys.Should().Contain($"Finance:Invoice:{invoice.Id}");
        cacheService.RemovedPrefixes.Should().Contain("Finance:Invoices");
    }

    [Fact]
    public async Task PayInvoiceCommandHandler_WithAlreadyPaidInvoice_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);
        invoice.AddLine("Item", 1, 100m);
        invoice.Send();
        invoice.Pay();

        var invoiceRepo = new FakeInvoiceRepository(invoice);
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new PayInvoiceCommandHandler(invoiceRepo, unitOfWork, cacheService);
        var command = new PayInvoiceCommand(invoice.Id);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleValidationException>().WithMessage("*already paid*");
    }

    [Fact]
    public async Task PayInvoiceCommandHandler_WithNonExistentInvoice_ReturnsFalse()
    {
        // Arrange
        var invoiceRepo = new FakeInvoiceRepository();
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var handler = new PayInvoiceCommandHandler(invoiceRepo, unitOfWork, cacheService);
        var command = new PayInvoiceCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateInvoiceCommandHandler_WithDraftInvoice_UpdatesDueDate()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);
        var invoiceRepo = new FakeInvoiceRepository(invoice);
        var unitOfWork = new FakeUnitOfWork();
        var cacheService = new FakeCacheService();

        var newDueDate = FutureDueDate.AddDays(10);
        var handler = new UpdateInvoiceCommandHandler(invoiceRepo, unitOfWork, cacheService);
        var command = new UpdateInvoiceCommand(invoice.Id, newDueDate);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        invoice.DueDate.Should().Be(newDueDate);
        unitOfWork.SaveCount.Should().Be(1);
    }

    // -----------------------------------------------------------------------
    // In-memory Test Doubles
    // -----------------------------------------------------------------------

    private sealed class FakeInvoiceRepository : IInvoiceRepository
    {
        public List<Invoice> Invoices { get; } = new();

        public FakeInvoiceRepository(params Invoice[] invoices)
        {
            Invoices.AddRange(invoices);
            // Set IDs for tests
            for (var i = 0; i < Invoices.Count; i++)
            {
                if (Invoices[i].Id == 0)
                {
                    typeof(Invoice).GetProperty(nameof(Invoice.Id))!.SetValue(Invoices[i], i + 1);
                }
            }
        }

        public Task<Invoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(Invoices.FirstOrDefault(x => x.Id == id));

        public Task<Invoice?> GetByIdWithLinesAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(Invoices.FirstOrDefault(x => x.Id == id));

        public Task<IEnumerable<Invoice>> GetAllAsync(QueryOptions<Invoice>? options = null, CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<Invoice>>(Invoices.ToList());

        public Task<PagedResult<Invoice>> GetPagedAsync(QueryOptions<Invoice>? options = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<Invoice>(Invoices.Skip((page - 1) * pageSize).Take(pageSize).ToList(), Invoices.Count, page, pageSize));

        public void Add(Invoice entity)
        {
            if (entity.Id == 0)
            {
                typeof(Invoice).GetProperty(nameof(Invoice.Id))!.SetValue(entity, Invoices.Count + 1);
            }
            Invoices.Add(entity);
        }

        public void Update(Invoice entity) { }
        public void Delete(Invoice entity) => Invoices.Remove(entity);
    }

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers = new();

        public FakeCustomerRepository(params Customer[] customers)
        {
            _customers.AddRange(customers);
            for (var i = 0; i < _customers.Count; i++)
            {
                if (_customers[i].Id == 0)
                {
                    typeof(Customer).GetProperty(nameof(Customer.Id))!.SetValue(_customers[i], i + 1);
                }
            }
        }

        public Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(_customers.FirstOrDefault(x => x.Id == id));

        public Task<Customer?> GetByIdWithOrdersAndInvoicesAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(_customers.FirstOrDefault(x => x.Id == id));

        public Task<IEnumerable<Customer>> GetAllAsync(QueryOptions<Customer>? options = null, CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<Customer>>(_customers.ToList());

        public Task<PagedResult<Customer>> GetPagedAsync(QueryOptions<Customer>? options = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<Customer>(_customers, _customers.Count, page, pageSize));

        public void Add(Customer entity) => _customers.Add(entity);
        public void Update(Customer entity) { }
        public void Delete(Customer entity) => _customers.Remove(entity);
    }

    private sealed class FakeOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders = new();

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
        public void Delete(Order entity) => _orders.Remove(entity);
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
