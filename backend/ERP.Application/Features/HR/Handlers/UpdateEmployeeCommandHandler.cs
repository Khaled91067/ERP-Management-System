namespace ERP.Application.Features.HR.Handlers;

using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.HR.Commands.Models;
using global::ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;

    public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id);
        if (employee is null)
            return false;

        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);
        if (department is null)
            throw new NotFoundException("Department", request.DepartmentId);

        employee.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.DepartmentId,
            request.Position,
            request.HireDate,
            request.Salary);

        _employeeRepository.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"HR:Employee:{employee.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync("HR:Employees", cancellationToken);

        return true;
    }
}


