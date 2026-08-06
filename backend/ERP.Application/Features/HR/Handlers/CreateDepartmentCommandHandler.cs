namespace ERP.Application.Features.HR.Handlers;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.HR.Commands;
using global::ERP.Domain.HR.Departments;
using global::ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, int>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentCommandHandler(
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;

    public async Task<int> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Department>();
        options.Filters.Add(d => d.Name == request.Name.Trim());

        var existing = await _departmentRepository.GetAllAsync(options);
        if (existing.Any())
            throw new ConflictException($"Department with name '{request.Name}' already exists.");

        var department = new Department(request.Name);

        _departmentRepository.Add(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveByPrefixAsync("HR:Departments", cancellationToken);

        return department.Id;
    }
}


