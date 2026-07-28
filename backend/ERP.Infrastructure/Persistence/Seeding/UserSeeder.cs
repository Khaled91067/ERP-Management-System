namespace ERP.Infrastructure.Persistence.Seeding;

using ERP.Application.Abstractions.Authentication;
using ERP.Domain.Identity.Users;
using ERP.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public class UserSeeder : ISeeder
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserSeeder(AppDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    private class UserSeedDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Users.AnyAsync(cancellationToken))
        {
            // If users already exist, just ensure the admin user exists
            var dtosFallback = await SeedHelper.ReadSeedDataAsync<UserSeedDto>("users.json");
            foreach (var dto in dtosFallback)
            {
                var targetEmail = new Email(dto.Email);
                if (!await _context.Users.AnyAsync(u => u.Email == targetEmail, cancellationToken))
                {
                    var roleFallback = await _context.Roles.FirstOrDefaultAsync(r => r.Name == dto.RoleName, cancellationToken);
                    if (roleFallback != null)
                    {
                        var hashFallback = _passwordHasher.Hash(dto.Password);
                        var userFallback = new User(dto.FirstName, dto.LastName, dto.Email, hashFallback, roleFallback.Id);
                        await _context.Users.AddAsync(userFallback, cancellationToken);
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }

        var dtos = await SeedHelper.ReadSeedDataAsync<UserSeedDto>("users.json");
        foreach (var dto in dtos)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == dto.RoleName, cancellationToken);
            if (role != null)
            {
                var hash = _passwordHasher.Hash(dto.Password);
                var user = new User(dto.FirstName, dto.LastName, dto.Email, hash, role.Id);
                await _context.Users.AddAsync(user, cancellationToken);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
