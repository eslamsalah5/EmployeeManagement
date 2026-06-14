using EmployeeManagement.API.Wrappers;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentDto>>>> GetAll()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(new ApiResponse<IEnumerable<DepartmentDto>>(departments, "Departments retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetById(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);

        if (department == null)
        {
            return NotFound(new ApiResponse<DepartmentDto>("Department not found"));
        }

        return Ok(new ApiResponse<DepartmentDto>(department, "Department retrieved successfully"));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Create([FromBody] CreateDepartmentDto createDto)
    {
        var department = await _departmentService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, 
            new ApiResponse<DepartmentDto>(department, "Department created successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var result = await _departmentService.DeleteAsync(id);

        if (!result)
        {
            return NotFound(new ApiResponse<object>("Department not found"));
        }

        return Ok(new ApiResponse<object>("Department deleted successfully"));
    }
}
