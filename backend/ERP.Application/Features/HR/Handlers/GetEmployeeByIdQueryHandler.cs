
namespace ERP.Application.Features.HR.Handlers;

using ERP.Application.Common.Caching;
using Microsoft.Extensions.Options;
using ERP.Application.Abstractions.Caching;
using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.HR.Dtos;
using ERP.Application.Features.HR.Queries;

using MediatR;

public sealed class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICacheService _cacheService;
    private readonly IOptions<CacheSettings> _cacheSettings;

    public GetEmployeeByIdQueryHandler(
        IEmployeeRepository employeeRepository,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings)
    {
        _employeeRepository = employeeRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"HR:Employee:{request.Id}";
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.FrequentDataExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var employee = await _employeeRepository.GetByIdWithDepartmentAsync(request.Id, cancellationToken);
                if (employee is null)
                    return null;

                return new EmployeeDto(
                    employee.Id,
                    employee.FirstName,
                    employee.LastName,
                    employee.Email.Value,
                    employee.Phone,
                    employee.DepartmentId,
                    employee.Department?.Name ?? string.Empty,
                    employee.Position,
                    employee.HireDate,
                    employee.Salary.Amount);
            },
            expiration,
            true,
            cancellationToken);
    }
}


