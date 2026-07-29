namespace ERP.Application.Features.Sales.Handlers;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Sales.Commands;
using global::ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;

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

        await _cacheService.RemoveAsync($"Sales:Customer:{customer.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync("Sales:Customers", cancellationToken);

        return true;
    }
}


