using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.HR.Commands.Models;
using ERP.Domain.Exceptions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.HR.Handlers;

public sealed class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id);
        if (employee is null)
            return false;

        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);
        if (department is null)
            throw new DomainException($"Department with ID {request.DepartmentId} was not found.");

        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = request.Email.Trim();
        employee.Phone = request.Phone.Trim();
        employee.DepartmentId = request.DepartmentId;
        employee.Position = request.Position.Trim();
        employee.HireDate = request.HireDate;
        employee.Salary = request.Salary;

        _employeeRepository.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
