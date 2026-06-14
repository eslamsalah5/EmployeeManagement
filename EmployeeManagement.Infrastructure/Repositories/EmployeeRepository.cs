using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Employee>> SearchAsync(string? searchTerm, int? departmentId)
    {
        var query = _dbSet.Include(e => e.Department).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e => e.FullName.Contains(searchTerm) || e.Email.Contains(searchTerm));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Employee?> GetByIdWithDepartmentAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
