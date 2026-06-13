using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Interfaces;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    // Additional department-specific methods can be added here
}
