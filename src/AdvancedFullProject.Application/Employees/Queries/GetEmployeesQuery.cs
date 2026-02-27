using AdvancedFullProject.Application.Common.Models;
using AdvancedFullProject.Application.Employees.DTOs;
using MediatR;

namespace AdvancedFullProject.Application.Employees.Queries;

public sealed record GetEmployeesQuery(PagedRequest Request) : IRequest<PagedResult<EmployeeDto>>;
