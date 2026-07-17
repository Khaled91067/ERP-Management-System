using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Authentication;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Identity.Commands;
using ERP.Application.Features.Identity.DTOs;
using ERP.Application.Features.Identity.Queries;
using ERP.Domain.Entities;
using MediatR;

namespace ERP.Application.Features.Identity.Handlers;

public sealed class GetUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllWithRolesAsync(cancellationToken);
        return users.Select(ToDto).ToList();
    }

    private static UserDto ToDto(User user) => new(user.Id, user.FirstName, user.LastName, user.Email,
        user.RoleId, user.Role?.Name ?? string.Empty);
}

public sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithRoleAsync(request.Id, cancellationToken);
        return user is null ? null : new UserDto(user.Id, user.FirstName, user.LastName, user.Email,
            user.RoleId, user.Role?.Name ?? string.Empty);
    }
}

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateUserCommand, int>
{
    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.EmailExistsAsync(request.Email.Trim(), cancellationToken))
            throw new InvalidOperationException("Email is already registered.");
        if (await roleRepository.GetByIdAsync(request.RoleId) is null)
            throw new InvalidOperationException("Role does not exist.");

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = passwordHasher.Hash(request.Password),
            RoleId = request.RoleId
        };
        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}

public sealed class UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, bool>
{
    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        if (user is null) return false;
        if (await userRepository.EmailExistsExceptAsync(request.Email.Trim(), user.Id, cancellationToken))
            throw new InvalidOperationException("Email is already registered.");

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = request.Email.Trim();
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class ChangeUserRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeUserRoleCommand, bool>
{
    public async Task<bool> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        if (user is null) return false;
        if (await roleRepository.GetByIdAsync(request.RoleId) is null)
            throw new InvalidOperationException("Role does not exist.");

        user.RoleId = request.RoleId;
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        if (user is null) return false;

        userRepository.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
