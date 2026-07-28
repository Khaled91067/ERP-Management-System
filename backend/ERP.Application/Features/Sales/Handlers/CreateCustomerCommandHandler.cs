namespace ERP.Application.Features.Sales.Handlers;

using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Commands.Models;
using ERP.Domain.Sales.Customers;

using MediatR;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

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

        return customer.Id;
    }
}
