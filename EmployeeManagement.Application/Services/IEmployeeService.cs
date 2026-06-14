using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync(string? search = null, int? departmentId = null);
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto createDto);
    Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto updateDto);
    Task<bool> DeleteAsync(int id);
}
