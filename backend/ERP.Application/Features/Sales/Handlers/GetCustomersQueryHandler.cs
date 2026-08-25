namespace ERP.Application.Features.Sales.Handlers;

using ERP.Application.Common.Caching;
using Microsoft.Extensions.Options;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Sales.Dtos;
using ERP.Application.Features.Sales.Queries;
using ERP.Domain.Sales.Customers;

using MediatR;

public sealed class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICacheService _cacheService;
    private readonly IOptions<CacheSettings> _cacheSettings;

    public GetCustomersQueryHandler(
        ICustomerRepository customerRepository,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings)
    {
        _customerRepository = customerRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<PagedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim() : "none";
        var cacheKey = $"Sales:Customers:Search={searchPart}:Page={request.Page}:Size={request.PageSize}";
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.PaginatedListExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<Customer> 
                { 
                    AsNoTracking = true,
                    OrderBy = q => System.Linq.Queryable.OrderBy(q, c => c.Name)
                };

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim();
                    options.Filters.Add(x => x.Name.Contains(search) ||
                                             x.Email.Value.Contains(search) ||
                                             x.Phone.Contains(search) ||
                                             x.TaxId.Contains(search) ||
                                             x.City.Contains(search));
                }

                var pagedCustomers = await _customerRepository.GetPagedAsync(options, request.Page, request.PageSize, ct);

                return pagedCustomers.Map(customer => new CustomerDto(
                    customer.Id,
                    customer.Name,
                    customer.Email.Value,
                    customer.Phone,
                    customer.Address,
                    customer.City,
                    customer.Country,
                    customer.TaxId));
            },
            expiration,
            false,
            cancellationToken);
    }
}


