using ERP.Application.Features.HR.Dtos;
using MediatR;

namespace ERP.Application.Features.HR.Queries.Models;

public sealed record GetEmployeeByIdQuery(
    int Id
) : IRequest<EmployeeDto?>;
