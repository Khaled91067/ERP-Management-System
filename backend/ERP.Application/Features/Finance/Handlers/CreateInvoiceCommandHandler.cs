namespace ERP.Application.Features.Finance.Handlers;

using System.Threading;
using System.Threading.Tasks;
using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Finance.Commands;
using global::ERP.Domain.Sales.Invoices;
using global::ERP.Domain.Shared.Exceptions;
using MediatR;

public sealed class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, int>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;
    private readonly IOrderRepository? _orderRepository;

    public CreateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService,
        IOrderRepository? orderRepository = null)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _orderRepository = orderRepository;
    }

    public async Task<int> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            throw new NotFoundException("Customer", request.CustomerId);

        if (request.OrderId.HasValue && _orderRepository is not null)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId.Value, cancellationToken);
            if (order is null)
                throw new NotFoundException("Order", request.OrderId.Value);

            if (order.CustomerId != request.CustomerId)
                throw new BusinessRuleValidationException("Order does not belong to the specified customer.");
        }

        if (request.Lines is null || request.Lines.Count == 0)
            throw new BusinessRuleValidationException("Invoice must have at least one line item.");

        var invoice = new Invoice(
            orderId: request.OrderId,
            customerId: request.CustomerId,
            dueDate: request.DueDate);

        foreach (var line in request.Lines)
        {
            invoice.AddLine(
                line.Description,
                line.Quantity,
                line.UnitPrice,
                line.TaxRate);
        }

        _invoiceRepository.Add(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveByPrefixAsync("Finance:Invoices", cancellationToken);

        return invoice.Id;
    }
}
