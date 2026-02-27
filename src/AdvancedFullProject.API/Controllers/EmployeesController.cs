using Asp.Versioning;
using AdvancedFullProject.Application.Common.Models;
using AdvancedFullProject.Application.Employees.Commands;
using AdvancedFullProject.Application.Employees.DTOs;
using AdvancedFullProject.Application.Employees.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedFullProject.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/employees")]
[Authorize(Policy = "CanReadEmployees")]
public sealed class EmployeesController : ControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 30)]
    public async Task<ActionResult<PagedResult<EmployeeDto>>> Get([FromQuery] PagedRequest request, ISender sender, CancellationToken cancellationToken)
        => Ok(await sender.Send(new GetEmployeesQuery(request), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeDto>> GetById(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var employee = await sender.Send(new GetEmployeeByIdQuery(id), cancellationToken);
        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeCommand command, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id, version = "1.0" }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeDto>> Update(Guid id, UpdateEmployeeCommand command, ISender sender, CancellationToken cancellationToken)
        => Ok(await sender.Send(command with { Id = id }, cancellationToken));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteEmployeeCommand(id, User.Identity?.Name ?? "system"), cancellationToken);
        return NoContent();
    }
}
