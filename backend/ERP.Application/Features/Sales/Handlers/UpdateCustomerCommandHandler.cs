namespace ERP.Application.Features.Sales.Handlers;

using ERP.Application.Common.Caching;
using ERP.Application.Abstractions.Caching;
using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Commands;

using MediatR;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, bool>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly ICacheService _cacheService;

    public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
            return false;

        customer.UpdateDetails(
            request.Name,
            request.Email,
            request.Phone,
            request.Address,
            request.City,
            request.Country,
            request.TaxId);

        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.Sales.CustomerById(customer.Id), cancellationToken);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.Sales.CustomersPrefix(), cancellationToken);

        return true;
    }
}


