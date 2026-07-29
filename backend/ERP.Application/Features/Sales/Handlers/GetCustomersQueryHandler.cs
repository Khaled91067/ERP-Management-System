namespace ERP.Application.Features.Sales.Handlers;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Sales.Dtos;
using global::ERP.Application.Features.Sales.Queries.Models;
using global::ERP.Domain.Sales.Customers;

using MediatR;

public sealed class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;
    private readonly global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> _cacheSettings;

    public GetCustomersQueryHandler(
        ICustomerRepository customerRepository,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService,
        global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> cacheSettings)
    {
        _customerRepository = customerRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<PagedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim().ToLower() : "none";
        var cacheKey = $"Sales:Customers:Search={searchPart}:Page={request.Page}:Size={request.PageSize}";
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.PaginatedListExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<Customer>();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLower();
                    options.Filter = x => x.Name.ToLower().Contains(search) ||
                                          x.Email.Value.ToLower().Contains(search) ||
                                          x.Phone.ToLower().Contains(search) ||
                                          x.TaxId.ToLower().Contains(search) ||
                                          x.City.ToLower().Contains(search);
                }

                var pagedCustomers = await _customerRepository.GetPagedAsync(options, request.Page, request.PageSize);

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


