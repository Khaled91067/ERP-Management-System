
namespace ERP.Application.Features.HR.Handlers;

using ERP.Application.Abstractions.Caching;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.HR.Commands;
using ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDepartmentCommandHandler(
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly ICacheService _cacheService;

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


