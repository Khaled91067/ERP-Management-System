
namespace ERP.Application.Features.HR.Queries;

using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.HR.Dtos;

using MediatR;

public sealed record GetDepartmentsQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<DepartmentDto>>;

