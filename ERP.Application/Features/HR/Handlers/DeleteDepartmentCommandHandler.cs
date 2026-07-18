using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.HR.Commands.Models;
using ERP.Domain.Exceptions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.HR.Handlers;

public sealed class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDepartmentCommandHandler(
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdWithEmployeesAsync(request.Id, cancellationToken);
        if (department is null)
            return false;

        // Verify if department is empty before allowing deletion
        if (department.Employees.Any())
        {
            throw new DomainException($"Cannot delete department '{department.Name}' because it has active employees.");
        }

        _departmentRepository.Delete(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
