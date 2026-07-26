using ERP.Application.Common.Models;
using ERP.Application.Features.HR.Dtos;
using MediatR;

namespace ERP.Application.Features.HR.Queries.Models;

public sealed record GetDepartmentsQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<DepartmentDto>>;
