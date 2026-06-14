using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Entities;
using Mapster;

namespace EmployeeManagement.Application.Mapping;

public static class MappingConfig
{
    public static void RegisterMaps()
    {
        // Department mappings
        TypeAdapterConfig<Department, DepartmentDto>.NewConfig();
        TypeAdapterConfig<CreateDepartmentDto, Department>.NewConfig()
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow);
    }
}
