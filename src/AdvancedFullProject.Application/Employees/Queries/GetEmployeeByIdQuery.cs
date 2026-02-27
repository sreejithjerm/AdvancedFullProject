using AdvancedFullProject.Application.Employees.DTOs;
using MediatR;

namespace AdvancedFullProject.Application.Employees.Queries;

public sealed record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDto?>;
