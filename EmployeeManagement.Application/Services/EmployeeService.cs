using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using Mapster;

namespace EmployeeManagement.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync(string? search = null, int? departmentId = null)
    {
        var employees = await _unitOfWork.Employees.SearchAsync(search, departmentId);
        return employees.Adapt<IEnumerable<EmployeeDto>>();
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdWithDepartmentAsync(id);
        return employee?.Adapt<EmployeeDto>();
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto createDto)
    {
        var employee = createDto.Adapt<Employee>();

        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        // Reload with department to populate DepartmentName
        var created = await _unitOfWork.Employees.GetByIdWithDepartmentAsync(employee.Id);
        return created!.Adapt<EmployeeDto>();
    }

    public async Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto updateDto)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
            return null;

        updateDto.Adapt(employee);

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();

        // Reload with department to populate DepartmentName
        var updated = await _unitOfWork.Employees.GetByIdWithDepartmentAsync(employee.Id);
        return updated!.Adapt<EmployeeDto>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
            return false;

        _unitOfWork.Employees.Delete(employee);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
