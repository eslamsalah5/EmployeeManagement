using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Infrastructure.Data;

namespace EmployeeManagement.Infrastructure.Repositories;

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext context) : base(context)
    {
    }
}
