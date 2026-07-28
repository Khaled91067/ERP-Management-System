namespace ERP.Application.Features.HR.Handlers;

using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.HR.Commands.Models;
using ERP.Domain.HR.Employees;
using ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, int>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);
        if (department is null)
            throw new NotFoundException("Department", request.DepartmentId);

        var employee = new Employee(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.DepartmentId,
            request.Position,
            request.HireDate,
            request.Salary);

        _employeeRepository.Add(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return employee.Id;
    }
}
