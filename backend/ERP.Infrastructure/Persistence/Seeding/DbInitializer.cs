namespace ERP.Infrastructure.Persistence.Seeding;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class DbInitializer
{
    private readonly AppDbContext _context;
    private readonly IEnumerable<ISeeder> _seeders;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(
        AppDbContext context,
        IEnumerable<ISeeder> seeders,
        ILogger<DbInitializer> logger)
    {
        _context = context;
        _seeders = seeders;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Migrating database...");
            await _context.Database.MigrateAsync(cancellationToken);

            _logger.LogInformation("Executing seeders...");
            foreach (var seeder in _seeders)
            {
                await seeder.SeedAsync(cancellationToken);
            }

            _logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }
}
