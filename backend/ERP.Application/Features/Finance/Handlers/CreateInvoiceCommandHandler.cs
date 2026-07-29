
namespace ERP.Application.Features.Finance.Handlers;


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

    public CreateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<int> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer is null)
            throw new NotFoundException("Customer", request.CustomerId);

        if (request.Lines is null || request.Lines.Count == 0)
            throw new BusinessRuleValidationException("Invoice must have at least one line item.");

        // For manual invoices not linked to an order, we create a placeholder order reference
        var invoice = new Invoice(
            orderId: 0,
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


