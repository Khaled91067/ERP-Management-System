namespace ERP.Application.Features.Sales.Handlers;

using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Sales.Commands.Models;
using global::ERP.Domain.Sales.Customers;

using MediatR;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;

    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer(
            request.Name,
            request.Email,
            request.Phone,
            request.Address,
            request.City,
            request.Country,
            request.TaxId);

        _customerRepository.Add(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveByPrefixAsync("Sales:Customers", cancellationToken);

        return customer.Id;
    }
}


