using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using Mapster;

namespace EmployeeManagement.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        var departments = await _unitOfWork.Departments.GetAllAsync();
        return departments.Adapt<IEnumerable<DepartmentDto>>();
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id);
        return department?.Adapt<DepartmentDto>();
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto createDto)
    {
        var department = createDto.Adapt<Department>();
        department.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Departments.AddAsync(department);
        await _unitOfWork.SaveChangesAsync();

        return department.Adapt<DepartmentDto>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id);
        if (department == null)
            return false;

        _unitOfWork.Departments.Delete(department);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
