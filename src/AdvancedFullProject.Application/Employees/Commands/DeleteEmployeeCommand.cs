using MediatR;

namespace AdvancedFullProject.Application.Employees.Commands;

public sealed record DeleteEmployeeCommand(Guid Id, string DeletedBy) : IRequest;
