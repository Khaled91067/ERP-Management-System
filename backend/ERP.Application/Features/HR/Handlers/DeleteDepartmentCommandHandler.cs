
namespace ERP.Application.Features.HR.Handlers;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.HR.Commands.Models;
using global::ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDepartmentCommandHandler(
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;

    public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdWithEmployeesAsync(request.Id, cancellationToken);
        if (department is null)
            return false;

        // Verify if department is empty before allowing deletion
        if (department.Employees.Any())
        {
            throw new BusinessRuleValidationException($"Cannot delete department '{department.Name}' because it has active employees.");
        }

        _departmentRepository.Delete(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveByPrefixAsync("HR:Departments", cancellationToken);

        return true;
    }
}


