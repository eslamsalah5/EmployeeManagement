using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Infrastructure.Data;

namespace EmployeeManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IEmployeeRepository? _employees;
    private IDepartmentRepository? _departments;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IEmployeeRepository Employees
    {
        get
        {
            _employees ??= new EmployeeRepository(_context);
            return _employees;
        }
    }

    public IDepartmentRepository Departments
    {
        get
        {
            _departments ??= new DepartmentRepository(_context);
            return _departments;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
