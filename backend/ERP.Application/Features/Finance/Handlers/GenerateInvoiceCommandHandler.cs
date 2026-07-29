namespace ERP.Application.Features.Finance.Handlers;

using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Finance.Commands.Models;
using global::ERP.Domain.Sales.Invoices;
using global::ERP.Domain.Sales.Orders;
using global::ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class GenerateInvoiceCommandHandler : IRequestHandler<GenerateInvoiceCommand, int>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GenerateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithLinesAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new NotFoundException("Order", request.OrderId);

        if (order.Status == OrderStatus.Cancelled)
            throw new BusinessRuleValidationException("Cannot generate an invoice for a cancelled order.");

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

        return invoice.Id;
    }
}

