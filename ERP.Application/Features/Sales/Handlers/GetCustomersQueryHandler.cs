using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Dtos;
using ERP.Application.Features.Sales.Queries.Models;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.Sales.Handlers;

public sealed class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync();

        // Apply Search Term filter in-memory if provided
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            customers = customers.Where(x => 
                x.Name.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search) ||
                x.Phone.ToLower().Contains(search) ||
                x.TaxId.ToLower().Contains(search) ||
                x.City.ToLower().Contains(search)
            ).ToList();
        }

        return customers.Select(customer => new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.Address,
            customer.City,
            customer.Country,
            customer.TaxId)).ToList();
    }
}
