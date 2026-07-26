using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Authentication;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Authentication.Commands.Models;
using ERP.Domain.Entities;
using MediatR;

namespace ERP.Application.Features.Authentication.Commands.Handlers;

public sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, int>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.EmailExistsAsync(request.Email,cancellationToken);

        if (emailExists)
            throw new InvalidOperationException("Email is already registered.");

        var passwordHash = _passwordHasher.Hash(request.Password);



        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = passwordHash,
            RoleId = 1 // Assuming 1 is the default role ID for a new user
        };

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}