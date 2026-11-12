namespace ERP.Infrastructure.Persistence.Seeding;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.HR.Employees;

using Microsoft.EntityFrameworkCore;

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
        public string DepartmentName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var dtos = await SeedHelper.ReadSeedDataAsync<EmployeeSeedDto>("employees.json");
        if (dtos.Count == 0) return;

        var existingEmails = await _context.Employees
            .Select(e => e.Email.Value)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<string>(existingEmails, StringComparer.OrdinalIgnoreCase);

        var departments = await _context.Departments
            .ToDictionaryAsync(d => d.Name, d => d.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var defaultDeptId = departments.Values.FirstOrDefault();
        var addedAny = false;

        foreach (var dto in dtos)
        {
            var trimmedEmail = dto.Email?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmedEmail) || existingSet.Contains(trimmedEmail))
            {
                continue;
            }

            var departmentId = 0;
            if (!string.IsNullOrWhiteSpace(dto.DepartmentName) && departments.TryGetValue(dto.DepartmentName.Trim(), out var resolvedDeptId))
            {
                departmentId = resolvedDeptId;
            }
            else if (dto.DepartmentId > 0 && departments.Values.Contains(dto.DepartmentId))
            {
                departmentId = dto.DepartmentId;
            }
            else
            {
                departmentId = defaultDeptId;
            }

            var emp = new Employee(
                dto.FirstName,
                dto.LastName,
                trimmedEmail,
                dto.Phone ?? string.Empty,
                departmentId,
                dto.Position ?? string.Empty,
                dto.HireDate == default ? DateTime.UtcNow.AddYears(-1) : dto.HireDate,
                dto.Salary);

            await _context.Employees.AddAsync(emp, cancellationToken);
            existingSet.Add(trimmedEmail);
            addedAny = true;
        }

        if (addedAny)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
