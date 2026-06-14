using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Interfaces;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<IEnumerable<Employee>> SearchAsync(string? searchTerm, int? departmentId);
    Task<Employee?> GetByIdWithDepartmentAsync(int id);
}
