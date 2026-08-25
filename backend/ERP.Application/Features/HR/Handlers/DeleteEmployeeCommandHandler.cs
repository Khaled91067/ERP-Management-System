
namespace ERP.Application.Features.HR.Handlers;

using ERP.Application.Abstractions.Caching;
using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.HR.Commands;

using MediatR;

public sealed class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly ICacheService _cacheService;

    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (employee is null)
            return false;

        _employeeRepository.Delete(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"HR:Employee:{employee.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync("HR:Employees", cancellationToken);

        return true;
    }
}


