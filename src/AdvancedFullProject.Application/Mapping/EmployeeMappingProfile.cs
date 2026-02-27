using AdvancedFullProject.Application.Employees.DTOs;
using AdvancedFullProject.Domain.Entities;
using AutoMapper;

namespace AdvancedFullProject.Application.Mapping;

public sealed class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<Employee, EmployeeDto>().ReverseMap();
    }
}
