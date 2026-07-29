
namespace ERP.Application.Features.HR.Handlers;

using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.HR.Dtos;
using global::ERP.Application.Features.HR.Queries.Models;

using MediatR;

public sealed class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;
    private readonly global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> _cacheSettings;

    public GetEmployeeByIdQueryHandler(
        IEmployeeRepository employeeRepository,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService,
        global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> cacheSettings)
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


