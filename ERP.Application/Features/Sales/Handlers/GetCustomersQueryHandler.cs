using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Sales.Dtos;
using ERP.Application.Features.Sales.Queries.Models;
using ERP.Domain.Entities;
using MediatR;

namespace ERP.Application.Features.Sales.Handlers;

public sealed class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<PagedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Customer>();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            options.Filter = x => x.Name.ToLower().Contains(search) ||
                                  x.Email.ToLower().Contains(search) ||
                                  x.Phone.ToLower().Contains(search) ||
                                  x.TaxId.ToLower().Contains(search) ||
                                  x.City.ToLower().Contains(search);
        }

        var pagedCustomers = await _customerRepository.GetPagedAsync(options, request.Page, request.PageSize);

        return pagedCustomers.Map(customer => new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.Address,
            customer.City,
            customer.Country,
            customer.TaxId));
    }
}
