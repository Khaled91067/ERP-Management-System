
namespace ERP.Application.Features.Sales.Handlers;

using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Sales.Dtos;
using global::ERP.Application.Features.Sales.Queries;

using MediatR;

public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;
    private readonly global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> _cacheSettings;

    public GetCustomerByIdQueryHandler(
        ICustomerRepository customerRepository,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService,
        global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> cacheSettings)
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
                var customer = await _customerRepository.GetByIdAsync(request.Id);
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


