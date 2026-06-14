using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
}
