using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.HR.Commands.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.HR.Handlers;

public sealed class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id);
        if (employee is null)
            return false;

        _employeeRepository.Delete(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
