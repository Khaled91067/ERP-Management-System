namespace ERP.Application.Features.HR.Handlers;

using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.HR.Commands;
using global::ERP.Domain.HR.Employees;
using global::ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, int>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeCommandHandler(
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

    public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
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

        await _cacheService.RemoveByPrefixAsync("HR:Employees", cancellationToken);

        return employee.Id;
    }
}


