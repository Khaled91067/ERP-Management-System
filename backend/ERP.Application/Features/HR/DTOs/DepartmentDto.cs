namespace ERP.Application.Features.HR.Dtos;

public sealed record DepartmentDto(
    int Id,
    string Name,
    int EmployeeCount);
