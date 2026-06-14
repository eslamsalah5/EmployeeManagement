using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto createDto);
    Task<bool> DeleteAsync(int id);
}
