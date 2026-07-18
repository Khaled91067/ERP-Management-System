using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Finance.Commands.Models;
using ERP.Domain.Entities;
using ERP.Domain.Exceptions;
using MediatR;

namespace ERP.Application.Features.Finance.Handlers;

public sealed class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, int>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer is null)
            throw new DomainException($"Customer with ID {request.CustomerId} was not found.");

        if (request.Lines is null || request.Lines.Count == 0)
            throw new DomainException("Invoice must have at least one line item.");

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

        return invoice.Id;
    }
}
