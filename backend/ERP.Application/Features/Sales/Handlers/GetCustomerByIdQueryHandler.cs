using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Dtos;
using ERP.Application.Features.Sales.Queries.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.Sales.Handlers;

public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);
        if (customer is null)
            return null;

        return new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.Address,
            customer.City,
            customer.Country,
            customer.TaxId);
    }
}
