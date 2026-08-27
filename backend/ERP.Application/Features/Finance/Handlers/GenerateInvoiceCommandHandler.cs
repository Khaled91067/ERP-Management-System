namespace ERP.Application.Features.Finance.Handlers;

using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Finance.Commands;
using ERP.Domain.Sales.Invoices;
using ERP.Domain.Sales.Orders;
using ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class GenerateInvoiceCommandHandler : IRequestHandler<GenerateInvoiceCommand, int>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public GenerateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _invoiceRepository = invoiceRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<int> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithLinesAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new NotFoundException("Order", request.OrderId);

        if (order.Status == OrderStatus.Cancelled)
            throw new BusinessRuleValidationException("Cannot generate an invoice for a cancelled order.");

        if (order.OrderLines.Count == 0)
            throw new BusinessRuleValidationException("Cannot generate an invoice for an order with no items.");

        // Create the Invoice aggregate
        var invoice = new Invoice(
            order.Id,
            order.CustomerId,
            request.DueDate);

        // Copy lines from the Order to the Invoice
        foreach (var line in order.OrderLines)
        {
            var productName = line.Product?.Name ?? "Product";
            var description = line.DiscountPercentage > 0
                ? $"{productName} (Discount: {line.DiscountPercentage}%)"
                : productName;

            // Calculate final price after discount
            var finalUnitPrice = line.UnitPrice.Amount * (1 - line.DiscountPercentage / 100);

            // Add line to invoice (assuming standard 0% tax by default)
            invoice.AddLine(description, line.Quantity, finalUnitPrice, taxRate: 0);
        }

        _invoiceRepository.Add(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveByPrefixAsync("Finance:Invoices", cancellationToken);

        return invoice.Id;
    }
}

