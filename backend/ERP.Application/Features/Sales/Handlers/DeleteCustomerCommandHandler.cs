namespace ERP.Application.Features.Sales.Handlers;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Commands.Models;
using ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdWithOrdersAndInvoicesAsync(request.Id, cancellationToken);
        if (customer is null)
            return false;

        if (customer.Orders.Any() || customer.Invoices.Any())
        {
            throw new BusinessRuleValidationException($"Cannot delete customer '{customer.Name}' because they have associated orders or invoices.");
        }

        _customerRepository.Delete(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
