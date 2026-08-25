namespace ERP.Application.Features.Sales.Handlers;

using ERP.Application.Abstractions.Caching;
using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Commands;
using ERP.Domain.Sales.Customers;

using MediatR;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly ICacheService _cacheService;

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


