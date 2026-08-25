
namespace ERP.Application.Features.Sales.Handlers;

using ERP.Application.Common.Caching;
using Microsoft.Extensions.Options;
using ERP.Application.Abstractions.Caching;
using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Dtos;
using ERP.Application.Features.Sales.Queries;

using MediatR;

public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICacheService _cacheService;
    private readonly IOptions<CacheSettings> _cacheSettings;

    public GetCustomerByIdQueryHandler(
        ICustomerRepository customerRepository,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings)
    {
        _customerRepository = customerRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"Sales:Customer:{request.Id}";
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.FrequentDataExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var customer = await _customerRepository.GetByIdAsync(request.Id, ct);
                if (customer is null)
                    return null;

                return new CustomerDto(
                    customer.Id,
                    customer.Name,
                    customer.Email.Value,
                    customer.Phone,
                    customer.Address,
                    customer.City,
                    customer.Country,
                    customer.TaxId);
            },
            expiration,
            true,
            cancellationToken);
    }
}


