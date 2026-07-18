using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.HR.Commands.Models;
using ERP.Domain.Entities;
using ERP.Domain.Exceptions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.HR.Handlers;

public sealed class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, int>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentCommandHandler(
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Department>
        {
            Filter = d => d.Name == request.Name.Trim()
        };

        var existing = await _departmentRepository.GetAllAsync(options);
        if (existing.Any())
            throw new DomainException($"Department with name '{request.Name}' already exists.");

        var department = new Department
        {
            Name = request.Name.Trim()
        };

        _departmentRepository.Add(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return department.Id;
    }
}
