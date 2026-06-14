using EmployeeManagement.API.Wrappers;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDto>>>> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] int? departmentId = null)
    {
        var employees = await _employeeService.GetAllAsync(search, departmentId);
        return Ok(new ApiResponse<IEnumerable<EmployeeDto>>(employees, "Employees retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetById(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);

        if (employee == null)
            return NotFound(new ApiResponse<EmployeeDto>("Employee not found"));

        return Ok(new ApiResponse<EmployeeDto>(employee, "Employee retrieved successfully"));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Create([FromBody] CreateEmployeeDto createDto)
    {
        var employee = await _employeeService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id },
            new ApiResponse<EmployeeDto>(employee, "Employee created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Update(int id, [FromBody] UpdateEmployeeDto updateDto)
    {
        var employee = await _employeeService.UpdateAsync(id, updateDto);

        if (employee == null)
            return NotFound(new ApiResponse<EmployeeDto>("Employee not found"));

        return Ok(new ApiResponse<EmployeeDto>(employee, "Employee updated successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var result = await _employeeService.DeleteAsync(id);

        if (!result)
            return NotFound(new ApiResponse<object>("Employee not found"));

        return Ok(new ApiResponse<object>("Employee deleted successfully"));
    }
}
