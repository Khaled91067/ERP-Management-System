namespace ERP.Infrastructure.Persistence.Seeding;

using ERP.Domain.HR.Employees;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

public class EmployeeSeeder : ISeeder
{
    private readonly AppDbContext _context;

    public EmployeeSeeder(AppDbContext context)
    {
        _context = context;
    }

    private class EmployeeSeedDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string Position { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Employees.AnyAsync(cancellationToken)) return;

        var dtos = await SeedHelper.ReadSeedDataAsync<EmployeeSeedDto>("employees.json");
        foreach (var dto in dtos)
        {
            var emp = new Employee(dto.FirstName, dto.LastName, dto.Email, dto.Phone, dto.DepartmentId, dto.Position, dto.HireDate, dto.Salary);
            await _context.Employees.AddAsync(emp, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
