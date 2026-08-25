namespace ERP.Application.Features.HR.Handlers;

using ERP.Application.Common.Caching;
using Microsoft.Extensions.Options;
using ERP.Application.Abstractions.Caching;
using System.Collections.Generic;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.HR.Dtos;
using ERP.Application.Features.HR.Queries;
using ERP.Domain.HR.Departments;

using MediatR;

public sealed class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, PagedResult<DepartmentDto>>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ICacheService _cacheService;
    private readonly IOptions<CacheSettings> _cacheSettings;

    public GetDepartmentsQueryHandler(
        IDepartmentRepository departmentRepository,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings)
    {
        _departmentRepository = departmentRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<PagedResult<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim() : "none";
        var cacheKey = $"HR:Departments:Search={searchPart}:Page={request.Page}:Size={request.PageSize}";
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.ReferenceDataExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<Department>
                {
                    AsNoTracking = true,
                    OrderBy = q => System.Linq.Queryable.OrderBy(q, d => d.Name),
                    Includes = new List<System.Linq.Expressions.Expression<System.Func<Department, object>>>
                    {
                        d => d.Employees
                    }
                };

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim();
                    options.Filters.Add(d => d.Name.Contains(search));
                }

                var pagedDepartments = await _departmentRepository.GetPagedAsync(options, request.Page, request.PageSize, ct);

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


