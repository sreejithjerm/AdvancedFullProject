using AdvancedFullProject.Application.Employees.DTOs;
using MediatR;

namespace AdvancedFullProject.Application.Employees.Commands;

public sealed record CreateEmployeeCommand(string FirstName, string LastName, string Email, string Department, decimal Salary) : IRequest<EmployeeDto>;
