namespace ERP.Application.Features.Identity.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Authentication;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Identity.Commands;
using global::ERP.Application.Features.Identity.DTOs;
using global::ERP.Application.Features.Identity.Queries;
using global::ERP.Domain.Identity.Users;

using MediatR;

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

        var user = new User(
            request.FirstName,
            request.LastName,
            request.Email,
            passwordHasher.Hash(request.Password),
            request.RoleId);

        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}

