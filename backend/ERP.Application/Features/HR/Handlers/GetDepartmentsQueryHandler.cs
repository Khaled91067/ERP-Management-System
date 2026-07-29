namespace ERP.Application.Features.HR.Handlers;

using System.Collections.Generic;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.HR.Dtos;
using global::ERP.Application.Features.HR.Queries;
using global::ERP.Domain.HR.Departments;

using MediatR;

public sealed class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, PagedResult<DepartmentDto>>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;
    private readonly global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> _cacheSettings;

    public GetDepartmentsQueryHandler(
        IDepartmentRepository departmentRepository,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService,
        global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> cacheSettings)
    {
        _departmentRepository = departmentRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<PagedResult<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim().ToLower() : "none";
        var cacheKey = $"HR:Departments:Search={searchPart}:Page={request.Page}:Size={request.PageSize}";
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.ReferenceDataExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<Department>
                {
                    Includes = new List<System.Linq.Expressions.Expression<System.Func<Department, object>>>
                    {
                        d => d.Employees
                    }
                };

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLower();
                    options.Filter = d => d.Name.ToLower().Contains(search);
                }

                var pagedDepartments = await _departmentRepository.GetPagedAsync(options, request.Page, request.PageSize);

                return pagedDepartments.Map(d => new DepartmentDto(
                    d.Id,
                    d.Name,
                    d.Employees.Count));
            },
            expiration,
            false,
            cancellationToken);
    }
}


