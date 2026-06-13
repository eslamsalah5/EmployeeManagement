namespace EmployeeManagement.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository Employees { get; }
    IDepartmentRepository Departments { get; }
    Task<int> SaveChangesAsync();
}
