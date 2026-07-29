namespace ERP.Application.Features.Authentication.Commands.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Authentication;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Authentication.Commands;
using global::ERP.Application.Features.Authentication.DTOs;
using global::ERP.Domain.Identity.Users;

using MediatR;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _iTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService jwtTokenGenerator, IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _iTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<TokenResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var passwordIsValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var accessToken = _iTokenGenerator.GenerateToken(user);
        var refreshToken = _iTokenGenerator.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(
            new RefreshToken(refreshToken, user.Id, DateTime.UtcNow.AddDays(7)),
            cancellationToken);

        await _unitOfWork.SaveChangesAsync();
        return new TokenResponse(accessToken, refreshToken);
    }
}

